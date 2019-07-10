namespace Asos.ThreatModelAutomation.Features.Domain
{
    public class EnumeratePasswordResult
    {
        public string Password { get; set; }
        public bool PasswordFound { get; set; }
        public int FailedRequestCounter { get; set; }
    }
}