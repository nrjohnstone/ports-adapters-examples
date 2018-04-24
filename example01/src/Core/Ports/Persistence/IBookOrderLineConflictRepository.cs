using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Ports.Persistence
{
    public interface IBookOrderLineConflictRepository
    {
        void Store(BookOrderLineConflict conflict);
        void Store(IEnumerable<BookOrderLineConflict> conflicts);
        BookOrderLineConflict Get(Guid bookOrderLineConflictId);
        IEnumerable<BookOrderLineConflict> Get();
    }
}