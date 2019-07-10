using System.Threading.Tasks;
using Asos.ThreatModelAutomation.Features.Domain;
using Asos.ThreatModelAutomation.Features.Helpers;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Asos.ThreatModelAutomation.Features.Definitions
{
    [Binding]
    public class CredentialStuffing
    {
        private readonly CredentialStuffingHelper Helper;
        private string EmailAddress;
        private EnumeratePasswordResult resultSet;

        public CredentialStuffing()
        {
            Helper = new CredentialStuffingHelper();
        }

        [Given(@"no controls")]
        public async Task GivenNoControls()
        {
            await Helper.SetFeatureSwitches(false, false);
            EmailAddress = await Helper.CreateUser();
        }

        [Given(@"Locking account after failed logins")]
        public async Task GivenLockingAccountAfterFailedLogins()
        {
            await Helper.SetFeatureSwitches(false, true);
            EmailAddress = await Helper.CreateUser();
        }

        [Given(@"No account creation with weak passwords")]
        public async Task GivenNoAccountCreationWithWeakPasswords()
        {
            await Helper.SetFeatureSwitches(true, false);
            EmailAddress = await Helper.CreateUser();
        }

        [When(@"I enumerate known passwords")]
        public async Task WhenIEnumerateKnownPasswords()
        {
            resultSet = await Helper.EnumeratePasswords(EmailAddress);
        }

        [Then(@"I gain access access a users account")]
        public void ThenIGainAccessAccessAUsersAccount()
        {
            if (resultSet.PasswordFound)
            {
                Assert.Pass($"Successful Brute force Credential match: {resultSet.Password}");
            }
            else if (resultSet.FailedRequestCounter > 0)
            {
                Assert.Inconclusive("Unexpected response code on one or more requests : " +  resultSet.FailedRequestCounter + " fails");
            }

            Assert.Fail("Unable to brute force account");
        }

        [Then(@"I don't gain access access a users account")]
        public void ThenIDonTGainAccessAccessAUsersAccount()
        {
            if (resultSet.PasswordFound)
            {
                Assert.Fail($"Successful Brute force Credential match: {resultSet.Password}");
            }
            else if (resultSet.FailedRequestCounter > 0)
            {
                Assert.Inconclusive("Unexpected response code on one or more requests : " + resultSet.FailedRequestCounter + " fails");
            }

            Assert.Pass("Unable to brute force account");
        }
    }
}
