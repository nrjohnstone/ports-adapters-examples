using System;
using System.Collections.Generic;
using System.Web.Http;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Host.WebService.Client1.BookOrders
{
    public class BookOrdersController : ApiController
    {
        private readonly OrderBookUseCase _orderBookUseCase;
        private readonly ApproveBookOrderUseCase _approveBookOrderUseCase;
        private readonly SendBookOrderUseCase _sendBookOrderUseCase;
        private readonly GetBookOrdersUseCase _getBookOrdersUseCase;

        public BookOrdersController(OrderBookUseCase orderBookUseCase,
            ApproveBookOrderUseCase approveBookOrderUseCase,
            SendBookOrderUseCase sendBookOrderUseCase,
            GetBookOrdersUseCase getBookOrdersUseCase)
        {
            if (orderBookUseCase == null) throw new ArgumentNullException(nameof(orderBookUseCase));
            if (approveBookOrderUseCase == null) throw new ArgumentNullException(nameof(approveBookOrderUseCase));
            if (sendBookOrderUseCase == null) throw new ArgumentNullException(nameof(sendBookOrderUseCase));
            if (getBookOrdersUseCase == null) throw new ArgumentNullException(nameof(getBookOrdersUseCase));
            _orderBookUseCase = orderBookUseCase;
            _approveBookOrderUseCase = approveBookOrderUseCase;
            _sendBookOrderUseCase = sendBookOrderUseCase;
            _getBookOrdersUseCase = getBookOrdersUseCase;
        }

        [HttpGet]
        [Route("health/instance")]
        public IHttpActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpPost]
        [Route("bookRequests")]
        public IHttpActionResult CreateBookRequest([FromBody] BookTitleOrderRequest bookTitleOrderRequest)
        {
            if (bookTitleOrderRequest == null)
                return BadRequest();

            Guid bookOrderId = _orderBookUseCase.Execute(new BookTitleOrder(
                bookTitleOrderRequest.Title, 
                bookTitleOrderRequest.Supplier,
                bookTitleOrderRequest.Price,
                bookTitleOrderRequest.Quantity ));
            
            return Created<string>($"bookOrders/{bookOrderId}", null);
        }

        [HttpGet]
        [Route("bookOrders")]
        public IHttpActionResult GetBookOrders()
        {
            var bookOrders = _getBookOrdersUseCase.Execute();

            IList<BookOrderResponse> bookOrdersResponse = new List<BookOrderResponse>();

            foreach (var bookOrder in bookOrders)
            {
                var orderLineResponses = new List<OrderLineResponse>();

                foreach (var orderLine in bookOrder.OrderLines)
                {
                    orderLineResponses.Add(new OrderLineResponse()
                    {
                        Title = orderLine.Title,
                        Price = orderLine.Price,
                        Quantity = orderLine.Quantity
                    });
                }

                var bookOrderResponse = new BookOrderResponse()
                {
                    Supplier = bookOrder.Supplier,
                    State = bookOrder.State.ToString(),
                    Id = bookOrder.Id.ToString(),
                    OrderLines = orderLineResponses
                };

                bookOrdersResponse.Add(bookOrderResponse);                
            }
                        
            return Ok(bookOrdersResponse);
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
}