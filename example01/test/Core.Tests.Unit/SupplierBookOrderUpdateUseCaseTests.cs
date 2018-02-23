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

            BookOrder bookOrder = new BookOrder(
                "SupplierBar",
                Guid.NewGuid(),
                BookOrderState.New);

            bookOrder.AddBookRequest(
                a.BookTitleOrder
                    .ForSupplier("SupplierBar")
                    .ForTitle("The Hobbit")
                    .WithPrice(10.50M)
            );

            _bookOrderRepository.Store(bookOrder);

            sut.Execute(new SupplierBookOrderUpdateRequest(
                bookOrder.Id,
                new List<SupplierBookOrderLineUpdateRequest>()
                {
                    new SupplierBookOrderLineUpdateRequest(
                        bookOrder.OrderLines[0].Id,
                        1000)
                }));

            List<BookOrderLineConflict> conflicts =
                _bookOrderLineConflictRepository.GetForBookOrder(bookOrder.Id).ToList();
            conflicts.Count().Should().Be(1);
            conflicts[0].ConflictType.Should().Be(ConflictType.Price);
        }
    }
}