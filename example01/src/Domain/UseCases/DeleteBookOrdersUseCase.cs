using System;
using Domain.Ports.Persistence;

namespace Domain.UseCases
{
    public class DeleteBookOrdersUseCase
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public DeleteBookOrdersUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null) throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public void Execute()
        {
            _bookOrderRepository.Delete();
        }
    }
}