using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Asos.ThreatModelAutomation.Features.Domain;
using NUnit.Framework;

namespace Asos.ThreatModelAutomation.Features.Helpers
{
    public class CredentialStuffingHelper
    {
        private const string LoginEndpoint = "http://localhost:2576/Login";
        private const string SetupEndpoint = "http://localhost:2576/Setup";
        private const string CreateUserEndpoint = "http://localhost:2576/Create";
        private readonly HttpClient _client;

        public CredentialStuffingHelper()
        {
            _client = new HttpClient();
        }

        public async Task<string> CreateUser()
        {
            var email = Guid.NewGuid().ToString("N").Substring(0, 10) + "@asos.com";

            var values = new Dictionary<string, string>
            {
                {"Email", email},
                {"Password", "Password1!"}
            };
            var content = new FormUrlEncodedContent(values);
            var response = await _client.PostAsync(CreateUserEndpoint, content);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                Assert.Inconclusive("Unable to create the user account");
            }

            return email;
        }

        public async Task SetFeatureSwitches(bool lockAccountEnabled, bool weakPasswordFilterEnabled)
        {
            var values = new Dictionary<string, string>
            {
                {"LockAccount", lockAccountEnabled.ToString()},
                {"WeakPasswordsAccount", weakPasswordFilterEnabled.ToString() }
            };
            var content = new FormUrlEncodedContent(values);

            var response = await _client.PostAsync(SetupEndpoint, content);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                Assert.Inconclusive("Unable to set the feature toggles");
            }
        }

        public async Task<EnumeratePasswordResult> EnumeratePasswords(string emailAddress)
        {
            IEnumerable<string> passwordList = GetPasswords(20);
            EnumeratePasswordResult resultSet = new EnumeratePasswordResult();
            foreach (string password in passwordList)
            {
                var values = new Dictionary<string, string>
                {
                    {"Email", emailAddress},
                    {"Password", password}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await _client.PostAsync(LoginEndpoint, content);

                var responseCode = response.StatusCode;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resultSet.Password = password;
                    resultSet.PasswordFound = true;
                    return resultSet;
                }

                if (responseCode == HttpStatusCode.Forbidden)
                {
                    resultSet.PasswordFound = false;
                }
                else if (responseCode != HttpStatusCode.OK)
                {
                    resultSet.FailedRequestCounter++;
                }
            }

            return resultSet;
        }

        private IEnumerable<string> GetPasswords(int numberOfPasswordsToReturn)
        {
            var list = new List<string>()
            {
                "123456",
                "12345",
                "123456789",
                "password",
                "iloveyou",
                "princess",
                "1234567",
                "12345678",
                "abc123",
                "nicole",
                "daniel",
                "babygirl",
                "monkey",
                "lovely",
                "jessica",
                "654321",
                "michael",
                "ashley",
                "Password1!",
                "qwerty"
            };
            return list.Take(numberOfPasswordsToReturn);
        }
    }
}