using System;
using System.Collections.Generic;
using Domain.UseCases;
using Domain.ValueObjects;
using HostApp.WebService.Client3.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HostApp.WebService.Client3.Controllers
{
    public class BookOrdersController : ControllerBase
    {
        private readonly AddBookTitleRequestUseCase _addBookTitleRequestUseCase;
        private readonly ApproveBookOrderUseCase _approveBookOrderUseCase;
        private readonly SendBookOrderUseCase _sendBookOrderUseCase;
        private readonly GetAllBookOrdersUseCase _getAllBookOrdersUseCase;
        private readonly DeleteBookOrdersUseCase _deleteBookOrdersUseCase;

        public BookOrdersController(AddBookTitleRequestUseCase addBookTitleRequestUseCase,
            ApproveBookOrderUseCase approveBookOrderUseCase,
            SendBookOrderUseCase sendBookOrderUseCase,
            GetAllBookOrdersUseCase getAllBookOrdersUseCase,
            DeleteBookOrdersUseCase deleteBookOrdersUseCase)
        {
            if (addBookTitleRequestUseCase == null) throw new ArgumentNullException(nameof(addBookTitleRequestUseCase));
            if (approveBookOrderUseCase == null) throw new ArgumentNullException(nameof(approveBookOrderUseCase));
            if (sendBookOrderUseCase == null) throw new ArgumentNullException(nameof(sendBookOrderUseCase));
            if (getAllBookOrdersUseCase == null) throw new ArgumentNullException(nameof(getAllBookOrdersUseCase));
            if (deleteBookOrdersUseCase == null) throw new ArgumentNullException(nameof(deleteBookOrdersUseCase));
            _addBookTitleRequestUseCase = addBookTitleRequestUseCase;
            _approveBookOrderUseCase = approveBookOrderUseCase;
            _sendBookOrderUseCase = sendBookOrderUseCase;
            _getAllBookOrdersUseCase = getAllBookOrdersUseCase;
            _deleteBookOrdersUseCase = deleteBookOrdersUseCase;
        }

        [HttpGet]
        [Route("health/instance")]
        public IStatusCodeActionResult HealthCheck()
        {
            return Ok(Program.ApplicationName);
        }

        [HttpPost]
        [Route("bookRequests")]
        public IStatusCodeActionResult CreateBookRequest([FromBody] BookTitleOrderRequest bookTitleOrderRequest)
        {
            if (bookTitleOrderRequest == null)
                return BadRequest();

            Guid bookOrderId = _addBookTitleRequestUseCase.Execute(new BookTitleRequest(
                bookTitleOrderRequest.Title,
                bookTitleOrderRequest.Supplier,
                bookTitleOrderRequest.Price,
                bookTitleOrderRequest.Quantity));

            return Created($"bookOrders/{bookOrderId}", null);
        }

        [HttpGet]
        [Route("bookOrders")]
        public IStatusCodeActionResult GetBookOrders()
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

        [HttpDelete]
        [Route("bookOrders")]
        public IStatusCodeActionResult DeleteBookOrders()
        {
            try
            {
                _deleteBookOrdersUseCase.Execute();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpPost]
        [Route("bookOrders/{bookOrderId}/approve")]
        public IStatusCodeActionResult ApproveBookOrder(Guid bookOrderId)
        {
            if (bookOrderId == Guid.Empty)
                return BadRequest();

            _approveBookOrderUseCase.Execute(bookOrderId);
            return Ok();
        }

        [HttpPost]
        [Route("bookOrders/{bookOrderId}/send")]
        public IStatusCodeActionResult SendBookOrder(Guid bookOrderId)
        {
            if (bookOrderId == Guid.Empty)
                return BadRequest();

            _sendBookOrderUseCase.Execute(bookOrderId);
            return Ok();
        }
    }
}