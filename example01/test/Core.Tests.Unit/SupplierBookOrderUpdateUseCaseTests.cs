using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adapter.Persistence.Test;
using Core.Tests.Unit.Helpers;
using Domain.Entities;
using Domain.UseCases;
using Domain.ValueObjects;
using Fluency.Utils;
using FluentAssertions;
using Xunit;

namespace Core.Tests.Unit
{
    public class SupplierBookOrderUpdateUseCaseTests
    {
        private readonly BookOrderRepository _bookOrderRepository;
        private readonly BookOrderLineConflictRepository _bookOrderLineConflictRepository;

        public SupplierBookOrderUpdateUseCaseTests()
        {
            _bookOrderRepository = new BookOrderRepository();
            _bookOrderLineConflictRepository = new BookOrderLineConflictRepository();
        }

        private SupplierBookOrderUpdateUseCase CreateSut()
        {
            var sut = new SupplierBookOrderUpdateUseCase(
                _bookOrderRepository,
                _bookOrderLineConflictRepository);
            return sut;
        }

        [Fact]
        public void WhenSupplierRequestsDifferentPrice_ShouldCreateBookOrderLineConflict()
        {
            var sut = CreateSut();

            BookOrder bookOrder = BookOrder.CreateNew(
                "SupplierBar",
                Guid.NewGuid());

            bookOrder.AddBookRequest(
                a.BookTitleOrder
                    .ForSupplier("SupplierBar")
                    .ForTitle("The Hobbit")
                    .WithQuantity(1)
                    .WithPrice(10.50M)
            );

            _bookOrderRepository.Store(bookOrder);

            sut.Execute(new SupplierBookOrderUpdateRequest(
                bookOrder.Id,
                new List<SupplierBookOrderLineUpdateRequest>()
                {
                    new SupplierBookOrderLineUpdateRequest(
                        bookOrder.OrderLines[0].Id,
                        price: 12.50M, quantity: 1)
                }));

            List<BookOrderLineConflict> conflicts =
                _bookOrderLineConflictRepository.GetForBookOrder(bookOrder.Id).ToList();
            conflicts.Count().Should().Be(1);
            conflicts[0].ConflictType.Should().Be(ConflictType.Price);
        }

        [Fact]
        public void WhenSupplierRequestsDifferentQuantity_ShouldCreateBookOrderLineConflict()
        {
            var sut = CreateSut();

            BookOrder bookOrder = BookOrder.CreateNew(
                "SupplierBar",
                Guid.NewGuid());

            bookOrder.AddBookRequest(
                a.BookTitleOrder
                    .ForSupplier("SupplierBar")
                    .ForTitle("The Hobbit")
                    .WithPrice(20)
                    .WithQuantity(10)
            );

            _bookOrderRepository.Store(bookOrder);

            var supplierBookOrderUpdateRequest = new SupplierBookOrderUpdateRequest(
                bookOrder.Id,
                new List<SupplierBookOrderLineUpdateRequest>()
                {
                    new SupplierBookOrderLineUpdateRequest(
                        bookOrder.OrderLines[0].Id,
                        price: 20, quantity: 9)
                });

            sut.Execute(supplierBookOrderUpdateRequest);

            List<BookOrderLineConflict> conflicts =
                _bookOrderLineConflictRepository.GetForBookOrder(bookOrder.Id).ToList();
            conflicts.Count().Should().Be(1);
            conflicts[0].ConflictType.Should().Be(ConflictType.Quantity);
        }
    }
}