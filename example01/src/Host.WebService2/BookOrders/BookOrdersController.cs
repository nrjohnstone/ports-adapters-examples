using System;
using System.Collections.Generic;
using System.Web.Http;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Host.WebService.Client2.BookOrders
{
    public class BookOrdersController : ApiController
    {
        private readonly ApproveBookOrderUseCase _approveBookOrderUseCase;
        private readonly SendBookOrderUseCase _sendBookOrderUseCase;
        private readonly GetBookOrdersUseCase _getBookOrdersUseCase;
        private readonly DeleteBookOrdersUseCase _deleteBookOrdersUseCase;

        public BookOrdersController(ApproveBookOrderUseCase approveBookOrderUseCase,
            SendBookOrderUseCase sendBookOrderUseCase,
            GetBookOrdersUseCase getBookOrdersUseCase,
            DeleteBookOrdersUseCase deleteBookOrdersUseCase)
        {
            if (approveBookOrderUseCase == null) throw new ArgumentNullException(nameof(approveBookOrderUseCase));
            if (sendBookOrderUseCase == null) throw new ArgumentNullException(nameof(sendBookOrderUseCase));
            if (getBookOrdersUseCase == null) throw new ArgumentNullException(nameof(getBookOrdersUseCase));
            if (deleteBookOrdersUseCase == null) throw new ArgumentNullException(nameof(deleteBookOrdersUseCase));
            _approveBookOrderUseCase = approveBookOrderUseCase;
            _sendBookOrderUseCase = sendBookOrderUseCase;
            _getBookOrdersUseCase = getBookOrdersUseCase;
            _deleteBookOrdersUseCase = deleteBookOrdersUseCase;
        }

        [HttpGet]
        [Route("health/instance")]
        public IHttpActionResult HealthCheck()
        {
            return Ok();
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

        [HttpDelete]
        [Route("bookOrders")]
        public IHttpActionResult DeleteBookOrders()
        {
            try
            {
                _deleteBookOrdersUseCase.Execute();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
            return Ok();
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