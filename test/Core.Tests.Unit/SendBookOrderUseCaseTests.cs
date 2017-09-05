﻿using System;
using Adapter.Persistence.Test;
using Core.Entities;
using Core.Ports.Notification;
using Core.Ports.Persistence;
using Core.Tests.Unit.Helpers;
using Core.UseCases;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Core.Tests.Unit
{
    public class SendBookOrderUseCaseTests
    {
        private readonly IBookOrderRepository _bookOrderRepository;
        private readonly IBookSupplierGateway _bookSupplierGateway;

        public SendBookOrderUseCaseTests()
        {
            _bookOrderRepository = new BookOrderRepository();
            _bookSupplierGateway = Substitute.For<IBookSupplierGateway>();
        }


        private SendBookOrderUseCase CreateSut()
        {
            return new SendBookOrderUseCase(_bookOrderRepository, _bookSupplierGateway);
        }

        [Fact]
        public void ApprovedBookOrder_ShouldBeSentToSupplierGateway()
        {
            var sut = CreateSut();
            BookOrder bookOrder = a.BookOrder.InState(BookOrderState.Approved);

            _bookOrderRepository.Store(bookOrder);

            sut.Execute(bookOrder.Id);

            _bookSupplierGateway.Received().Send(bookOrder);
        }

        [Theory]
        [InlineData(BookOrderState.New)]
        [InlineData(BookOrderState.Sent)]
        public void SendingABookOrder_WhenStateIsNotApproved_ShouldThrowDomainException(BookOrderState state)
        {
            var sut = CreateSut();
            BookOrder bookOrder = a.BookOrder.InState(state);
            _bookOrderRepository.Store(bookOrder);

            Action sendBookOrderThatIsAlreadySent = () => sut.Execute(bookOrder.Id);

            sendBookOrderThatIsAlreadySent.ShouldThrow<Exception>();
        }

    }
}