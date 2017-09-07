using System;
using System.Web.Http;
using Core.Ports.Persistence;
using Core.UseCases;
using Core.ValueObjects;

namespace Host.WebService1
{
    public class BookOrdersController : ApiController
    {
        private readonly OrderBookUseCase _orderBookUseCase;
        
        public BookOrdersController(OrderBookUseCase orderBookUseCase)
        {
            if (orderBookUseCase == null) throw new ArgumentNullException(nameof(orderBookUseCase));
            _orderBookUseCase = orderBookUseCase;
        }

        [HttpGet]
        [Route("health/instance")]
        public IHttpActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpPost]
        [Route("bookRequests")]
        public IHttpActionResult CreateBookRequest([FromBody] BookRequestDto bookRequestDto)
        {
            if (bookRequestDto == null)
                return BadRequest();

            _orderBookUseCase.Execute(new BookRequest(
                bookRequestDto.Title, 
                bookRequestDto.Supplier,
                bookRequestDto.Price,
                bookRequestDto.Quantity ));

            return Created<string>("", "");
        }
    }

    public class BookRequestDto
    {
        public string Title { get; set; }
        public string Supplier { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}