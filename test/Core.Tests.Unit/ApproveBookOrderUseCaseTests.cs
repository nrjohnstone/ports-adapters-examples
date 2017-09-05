using System;
using Adapter.Persistence.Test;
using Core.Entities;
using Core.Ports.Persistence;
using Core.Tests.Unit.Helpers;
using Core.UseCases;
using FluentAssertions;
using Xunit;

namespace Core.Tests.Unit
{
    public class ApproveBookOrderUseCaseTests
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public ApproveBookOrderUseCaseTests()
        {
            _bookOrderRepository = new BookOrderRepository();
        }

        private ApproveBookOrderUseCase CreateSut()
        {
            return new ApproveBookOrderUseCase(_bookOrderRepository);
        }

        [Fact]
        public void NewBookOrder_ShouldBeApproved()
        {
            var sut = CreateSut();
            BookOrder bookOrder = a.BookOrder.InState(BookOrderState.New);
            _bookOrderRepository.Store(bookOrder);

            sut.Execute(bookOrder.Id);

            var storedBookOrder = _bookOrderRepository.Get(bookOrder.Id);
            storedBookOrder.State.Should().Be(BookOrderState.Approved);
        }

        [Theory]
        [InlineData(BookOrderState.Approved)]
        [InlineData(BookOrderState.Sent)]
        public void ApproveABookOrder_WhenStateIsNotNew_ShouldThrowDomainException(BookOrderState state)
        {
            var sut = CreateSut();

            BookOrder bookOrder = a.BookOrder.InState(state);
            _bookOrderRepository.Store(bookOrder);

            Action sendBookOrderThatIsAlreadySent = () => sut.Execute(bookOrder.Id);

            sendBookOrderThatIsAlreadySent.ShouldThrow<Exception>();
        }
    }
}