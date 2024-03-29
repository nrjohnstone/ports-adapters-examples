﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Ports.Persistence;
using Domain.ValueObjects;
using Serilog;

namespace Domain.UseCases
{
    public class AddBookTitleRequestUseCase
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public AddBookTitleRequestUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null)
                throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public Guid Execute(BookTitleRequest bookTitleRequest)
        {
            Log.Logger.Information($"Execute {nameof(AddBookTitleRequestUseCase)} for Title: {{Title}}", bookTitleRequest.Title);

            // Check for any existing new orders for the supplier
            IEnumerable<BookOrder> bookOrders = _bookOrderRepository.GetBySupplier(
                bookTitleRequest.Supplier, BookOrderState.New);
            
            var bookOrder = bookOrders.FirstOrDefault();

            // Create a new book order if none already exists for this supplier
            if (bookOrder == null)
            {
                bookOrder = BookOrder.CreateNew(
                    bookTitleRequest.Supplier, Guid.NewGuid());
            }

            bookOrder.AddBookRequest(bookTitleRequest);

            _bookOrderRepository.Store(bookOrder);

            return bookOrder.Id;
        }
    }
}