using System;
using System.Collections.Generic;
using System.Web.Http;
using AmbientContext.LogService.Serilog;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Host.WebService.Client1.BookOrders
{
    public class BookOrdersController : ApiController
    {
        public AmbientLogService Log { get; set; }
        private readonly AddBookTitleRequestUseCase _addBookTitleRequestUseCase;
        private readonly ApproveBookOrderUseCase _approveBookOrderUseCase;
        private readonly SendBookOrderUseCase _sendBookOrderUseCase;
        private readonly GetAllBookOrdersUseCase _getAllBookOrdersUseCase;
        private readonly DeleteBookOrdersUseCase _deleteBookOrdersUseCase;
        private readonly GetAllNewBookOrdersUseCase _getAllNewBookOrdersUseCase;
        private readonly SupplierBookOrderUpdateUseCase _supplierBookOrderUpdateUseCase;

        public BookOrdersController(AddBookTitleRequestUseCase addBookTitleRequestUseCase,
            ApproveBookOrderUseCase approveBookOrderUseCase,
            SendBookOrderUseCase sendBookOrderUseCase,
            GetAllBookOrdersUseCase getAllBookOrdersUseCase,
            DeleteBookOrdersUseCase deleteBookOrdersUseCase,
            GetAllNewBookOrdersUseCase getAllNewBookOrdersUseCase,
            SupplierBookOrderUpdateUseCase supplierBookOrderUpdateUseCase)
        {
            if (supplierBookOrderUpdateUseCase == null) throw new ArgumentNullException(nameof(supplierBookOrderUpdateUseCase));
            if (addBookTitleRequestUseCase == null) throw new ArgumentNullException(nameof(addBookTitleRequestUseCase));
            if (approveBookOrderUseCase == null) throw new ArgumentNullException(nameof(approveBookOrderUseCase));
            if (sendBookOrderUseCase == null) throw new ArgumentNullException(nameof(sendBookOrderUseCase));
            if (getAllBookOrdersUseCase == null) throw new ArgumentNullException(nameof(getAllBookOrdersUseCase));
            if (deleteBookOrdersUseCase == null) throw new ArgumentNullException(nameof(deleteBookOrdersUseCase));
            if (getAllNewBookOrdersUseCase == null) throw new ArgumentNullException(nameof(getAllNewBookOrdersUseCase));

            _addBookTitleRequestUseCase = addBookTitleRequestUseCase;
            _approveBookOrderUseCase = approveBookOrderUseCase;
            _sendBookOrderUseCase = sendBookOrderUseCase;
            _getAllBookOrdersUseCase = getAllBookOrdersUseCase;
            _deleteBookOrdersUseCase = deleteBookOrdersUseCase;
            _getAllNewBookOrdersUseCase = getAllNewBookOrdersUseCase;
            _supplierBookOrderUpdateUseCase = supplierBookOrderUpdateUseCase;
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

            Guid bookOrderId = _addBookTitleRequestUseCase.Execute(new BookTitleRequest(
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
            var bookOrders = _getAllBookOrdersUseCase.Execute();

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

        [HttpGet]
        [Route("bookOrders/new")]
        public IHttpActionResult GetNewBookOrders()
        {
            var bookOrders = _getAllNewBookOrdersUseCase.Execute();

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
            catch (Exception ex)
            {
                Log.Error(ex ,"Exception occurred while deleting book orders");
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

        [HttpPost]
        [Route("bookOrders/{bookOrderId}/supplierUpdate")]
        public IHttpActionResult ReceiveSupplierUpdate([FromUri] Guid bookOrderId, [FromBody] SupplierBookOrderUpdateDto dto)
        {
            List<SupplierBookOrderLineUpdateRequest> orderlineUpdates = new List<SupplierBookOrderLineUpdateRequest>();

            foreach (var supplierBookOrderLineUpdateDto in dto.OrderLineUpdates)
            {
                orderlineUpdates.Add(new SupplierBookOrderLineUpdateRequest(
                    supplierBookOrderLineUpdateDto.BookOrderLineId, supplierBookOrderLineUpdateDto.Price,
                    supplierBookOrderLineUpdateDto.Quantity));
            }
            SupplierBookOrderUpdateRequest supplierBookOrderUpdateRequest =
                new SupplierBookOrderUpdateRequest(bookOrderId, orderlineUpdates);

            _supplierBookOrderUpdateUseCase.Execute(supplierBookOrderUpdateRequest);
            return Ok();
        }
    }
}