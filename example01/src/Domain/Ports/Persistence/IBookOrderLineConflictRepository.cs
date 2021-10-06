using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Ports.Persistence
{
    public interface IBookOrderLineConflictRepository
    {
        void Store(BookOrderLineConflict conflict);
        void Store(IEnumerable<BookOrderLineConflict> conflict);
        BookOrderLineConflict Get(Guid id);
        IEnumerable<BookOrderLineConflict> Get();
    }
}