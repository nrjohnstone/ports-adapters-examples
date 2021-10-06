using System;
using Adapter.Notification.InMemory;
using Adapter.Persistence.Test;
using Core.Tests.Unit.Helpers;
using Domain.Entities;
using Domain.Ports.Persistence;
using Domain.UseCases;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Core.Tests.Unit
{
    public class SendBookOrderUseCaseTests
    {
        private readonly IBookOrderRepository _bookOrderRepository;
        private readonly BookSupplierGatewayInMemory _bookSupplierGatewayInMemory;

        public SendBookOrderUseCaseTests()
        {
            _bookOrderRepository = new BookOrderRepository();
            _bookSupplierGatewayInMemory = new BookSupplierGatewayInMemory();
        }

        private SendBookOrderUseCase CreateSut()
        {
            return new SendBookOrderUseCase(_bookOrderRepository, _bookSupplierGatewayInMemory);
        }

        [Fact]
        public void ApprovedBookOrder_ShouldBeSentToSupplierGateway()
        {
            var sut = CreateSut();
            BookOrder bookOrder = a.BookOrder.ThatIsApproved();

            _bookOrderRepository.Store(bookOrder);

            sut.Execute(bookOrder.Id);

            _bookSupplierGatewayInMemory.SentBookOrders.Should().Contain(bookOrder);
        }

        [Theory]
        [InlineData(BookOrderState.New)]
        [InlineData(BookOrderState.Sent)]
        public void SendingABookOrder_WhenStateIsNotApproved_ShouldThrowDomainException(BookOrderState state)
        {
            var sut = CreateSut();
            BookOrder bookOrder = a.BookOrder.WithState(state);
            _bookOrderRepository.Store(bookOrder);

            Action sendBookOrderThatIsAlreadySent = () => sut.Execute(bookOrder.Id);

            sendBookOrderThatIsAlreadySent.ShouldThrow<Exception>();
        }

    }
}