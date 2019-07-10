using System.Net;
using System.Net.Http;
using System.Web.Http;
using Asos.ThreatModelAutomation.Web.Models;
using Asos.ThreatModelAutomation.Web.Repository;

namespace Asos.ThreatModelAutomation.Web.Controllers
{
    public class AllEndpointsController : ApiController
    {
        private readonly IRepository _repo;
        public AllEndpointsController()
        {
            _repo = new MemCacheRepo();
        }

        [HttpPost]
        [Route("setup")]
        public HttpResponseMessage Setup([FromBody]SetupRequest payload)
        {
            _repo.SetUpFeatures(payload.LockAccount, payload.WeakPasswordsAccount);
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Login([FromBody]LoginRequest payload)
        {
            var responseCode = _repo.Login(payload.Email, payload.Password) ? HttpStatusCode.OK : HttpStatusCode.Forbidden;
            return new HttpResponseMessage(responseCode);
        }

        [HttpPost]
        [Route("create")]
        public HttpResponseMessage Create([FromBody]CreateRequest payload)
        {
            _repo.Create(payload.Email, payload.Password);
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
