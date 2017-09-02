using Core.Entities;
using Core.Ports.Persistence;
using Core.UseCases;
using Core.ValueObjects;
using NSubstitute;
using Xunit;

namespace Core.Tests.Unit
{
    public class OrderBookUseCaseTests
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public OrderBookUseCaseTests()
        {
            _bookOrderRepository = Substitute.For<IBookOrderRepository>();
        }

        [Fact]
        public void OrderingABook_WhenOrderForSupplierDoesNotExist_ShouldCreateNewOrderForSupplier()
        {
            var sut = CreateSut();

            sut.Execute(new BookRequest(
                "Foo", "SupplierFoo", 10.5M, 1));

            _bookOrderRepository.Received().Store(Arg.Is<BookOrder>(
                storedBook => storedBook.Supplier == "SupplierFoo"));
        }

        private OrderBookUseCase CreateSut()
        {
            return new OrderBookUseCase(
                _bookOrderRepository);
        }
    }
}