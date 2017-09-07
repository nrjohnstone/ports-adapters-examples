using System;
using System.Web.Http;
using Core.Ports.Persistence;

namespace Host.WebService1
{
    public class BookOrdersController : ApiController
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public BookOrdersController(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null) throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        [HttpGet, Route("health/instance")]
        public IHttpActionResult HealthCheck()
        {
            return Ok();
        }
    }
}