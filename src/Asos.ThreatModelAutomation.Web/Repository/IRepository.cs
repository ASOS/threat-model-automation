namespace Asos.ThreatModelAutomation.Web.Repository
{
    public interface IRepository
    {
        bool Login(string payloadEmail, string payloadPassword);
        void Create(string payloadEmail, string payloadPassword);
        void SetUpFeatures(bool lockAccountEnable, bool weakPasswordFilterEnable);
    }
}