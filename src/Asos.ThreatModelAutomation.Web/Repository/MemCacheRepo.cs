using System.Collections.Generic;
using System.Runtime.Caching;
using Asos.ThreatModelAutomation.Web.Models;

namespace Asos.ThreatModelAutomation.Web.Repository
{
    public class MemCacheRepo : IRepository
    {
        private readonly ObjectCache _cache;
        private bool _isLockOutEnabled;
        private bool _isWeakPasswordFilterEnabled;

        public MemCacheRepo()
        {
            _cache = MemoryCache.Default;

            if (_cache[CacheKeys.LockOut.ToString()] == null)
            {
                _cache[CacheKeys.LockOut.ToString()] = false;
            }
            if (_cache[CacheKeys.WeakPasswordFilter.ToString()] == null)
            {
                _cache[CacheKeys.WeakPasswordFilter.ToString()] = false;
            }
            _isLockOutEnabled = bool.Parse(_cache[CacheKeys.LockOut.ToString()].ToString());
            _isWeakPasswordFilterEnabled = bool.Parse(_cache[CacheKeys.WeakPasswordFilter.ToString()].ToString());
        }

        public bool Login(string email, string password)
        {
            var accountDataSet = _cache[CacheKeys.DataSet.ToString()] as List<CredentialRecord>;
            if (accountDataSet == null) return false;
            foreach (var credentialRecord in accountDataSet)
            {
                if (credentialRecord.Email == email)
                {
                    if (credentialRecord.FailCounter > 5)
                    {
                        return false;
                    }

                    if (credentialRecord.Password != password && _isLockOutEnabled)
                    {
                        credentialRecord.FailCounter += 1;
                        _cache[CacheKeys.DataSet.ToString()] = accountDataSet;
                    }

                    if (credentialRecord.Password == password)
                    {
                        return true;
                    }

                    return false;
                }
            }
            return false;
        }

        public void Create(string email, string password)
        {
            if (_isWeakPasswordFilterEnabled)
            {
                return;
            }
            var accountDataSet = _cache[CacheKeys.DataSet.ToString()] as List<CredentialRecord> ?? new List<CredentialRecord>();
            accountDataSet.Add(new CredentialRecord(){Email = email, Password = password});
            _cache[CacheKeys.DataSet.ToString()] = accountDataSet;
        }

        public void SetUpFeatures(bool lockAccountEnable, bool weakPasswordFilterEnable)
        {
            SetLockOut(lockAccountEnable);
            SetNoWeakPassword(weakPasswordFilterEnable);
        }

        private void SetNoWeakPassword(bool enableWeakPasswordFilter)
        {
            _isWeakPasswordFilterEnabled = enableWeakPasswordFilter;
            _cache[CacheKeys.WeakPasswordFilter.ToString()] = enableWeakPasswordFilter;
        }

        private void SetLockOut(bool enableLockAccount)
        {
            _isLockOutEnabled = enableLockAccount;
            _cache[CacheKeys.LockOut.ToString()] = enableLockAccount;
            if (enableLockAccount == false)
            {
                var accountDataSet = _cache[CacheKeys.DataSet.ToString()] as List<CredentialRecord>;
                if (accountDataSet == null) return;
                foreach (var credentialRecord in accountDataSet)
                {
                    credentialRecord.FailCounter = 0;
                }
                _cache[CacheKeys.DataSet.ToString()] = accountDataSet;
            }
        }

        private enum CacheKeys
        {
            DataSet,
            LockOut,
            WeakPasswordFilter
        }
    }
}