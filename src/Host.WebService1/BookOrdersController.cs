using System;
using System.Linq;
using System.Web.Http;
using Core.Ports.Persistence;
using Core.UseCases;
using Core.ValueObjects;

namespace Host.WebService1
{
    public class BookOrdersController : ApiController
    {
        private readonly OrderBookUseCase _orderBookUseCase;
        private readonly ApproveBookOrderUseCase _approveBookOrderUseCase;
        private readonly SendBookOrderUseCase _sendBookOrderUseCase;

        public BookOrdersController(OrderBookUseCase orderBookUseCase,
            ApproveBookOrderUseCase approveBookOrderUseCase,
            SendBookOrderUseCase sendBookOrderUseCase)
        {
            if (orderBookUseCase == null) throw new ArgumentNullException(nameof(orderBookUseCase));
            if (approveBookOrderUseCase == null) throw new ArgumentNullException(nameof(approveBookOrderUseCase));
            if (sendBookOrderUseCase == null) throw new ArgumentNullException(nameof(sendBookOrderUseCase));
            _orderBookUseCase = orderBookUseCase;
            _approveBookOrderUseCase = approveBookOrderUseCase;
            _sendBookOrderUseCase = sendBookOrderUseCase;
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

            Guid bookOrderId = _orderBookUseCase.Execute(new BookRequest(
                bookRequestDto.Title, 
                bookRequestDto.Supplier,
                bookRequestDto.Price,
                bookRequestDto.Quantity ));
            
            return Created<string>($"bookOrders/{bookOrderId}", null);
        }
        
        [HttpPost]
        [Route("bookOrders/{bookOrderId}/approve")]
        public IHttpActionResult ApproveBookOrder(Guid bookOrderId)
        {
            if (bookOrderId == Guid.Empty)
                return BadRequest();

            _approveBookOrderUseCase.Execute(bookOrderId);
            return Ok();
        }

        [HttpPost]
        [Route("bookOrders/{bookOrderId}/send")]
        public IHttpActionResult SendBookOrder(Guid bookOrderId)
        {
            if (bookOrderId == Guid.Empty)
                return BadRequest();

            _sendBookOrderUseCase.Execute(bookOrderId);
            return Ok();
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