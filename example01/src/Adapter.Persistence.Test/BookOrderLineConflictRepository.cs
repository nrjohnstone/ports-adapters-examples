using System;
using System.Collections.Generic;
using System.Configuration;
using Domain.Entities;
using Domain.Ports.Persistence;

namespace Adapter.Persistence.Test
{
    public class BookOrderLineConflictRepository : IBookOrderLineConflictRepository
    {
        public readonly Dictionary<Guid, BookOrderLineConflict> Data = new Dictionary<Guid, BookOrderLineConflict>();

        public void Store(BookOrderLineConflict conflict)
        {
            Data[conflict.Id] = conflict;
        }

        public void Store(IEnumerable<BookOrderLineConflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                Data[conflict.Id] = conflict;
            }
        }

        public BookOrderLineConflict Get(Guid id)
        {
            if (Data.ContainsKey(id))
                return Data[id];

            return null;
        }

        public IEnumerable<BookOrderLineConflict> Get()
        {
            return Data.Values;
        }

        public IEnumerable<BookOrderLineConflict> GetForBookOrder(Guid bookOrderId)
        {
            List<BookOrderLineConflict> conflicts = new List<BookOrderLineConflict>();
            foreach (var bookOrderLineConflict in Data.Values)
            {
                if (bookOrderLineConflict.BookOrderId == bookOrderId)
                    conflicts.Add(bookOrderLineConflict);
            }


            return conflicts;
        }
    }
}