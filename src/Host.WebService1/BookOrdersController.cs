using System.Web.Http;

namespace Host.WebService1
{
    public class BookOrdersController : ApiController
    {
        [HttpGet, Route("health/instance")]
        public IHttpActionResult HealthCheck()
        {
            return Ok();
        }
    }
}