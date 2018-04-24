using System;
using System.Collections.Generic;
using Adapter.Persistence.CouchDb.Repositories.Dtos;
using Domain.Entities;
using Domain.Ports.Persistence;
using MyCouch;
using Newtonsoft.Json;

namespace Adapter.Persistence.CouchDb.Repositories
{
    public class BookOrderLineConflictRepository : IBookOrderLineConflictRepository
    {
        private readonly string _databaseUri;
        private readonly string _databaseName;

        public BookOrderLineConflictRepository(CouchDbSettings settings)
        {
            _databaseUri = settings.DatabaseUri;
            _databaseName = settings.DatabaseName;
        }

        public void Store(BookOrderLineConflict conflict)
        {
            using (var store = new MyCouchStore(_databaseUri, _databaseName))
            {
                // HACK: Concurrency support, lets just get the latest rev
                // Don't do this in production environments !! Implement concurrency properly all the way
                // to the client !!
                string rev = GetRev(store, conflict.Id);

                BookOrderLineConflictDto dto = new BookOrderLineConflictDto()
                {
                    _id = conflict.Id.ToString(),
                    BookOrderId = conflict.BookOrderId,
                    BookOrderLineId = conflict.BookOrderLineId
                };

                if (rev == null)
                {
                    store.StoreAsync(dto).Wait();
                }
                else
                {
                    dto._rev = rev;
                    store.StoreAsync(dto).Wait();
                }
            }
        }

        private string GetRev(MyCouchStore store, Guid id)
        {
            var doc = store.GetByIdAsync(id.ToString()).Result;

            if (doc == null)
                return null;

            var retrieved = store.GetByIdAsync<BookOrderLineConflictDto>(id.ToString()).Result;
            return retrieved._rev;
        }

        public void Store(IEnumerable<BookOrderLineConflict> conflict)
        {
            throw new NotImplementedException();
        }

        public BookOrderLineConflict Get(Guid id)
        {
            BookOrderLineConflictDto dto;

            using (var store = new MyCouchStore(_databaseUri, _databaseName))
            {
                var doc = store.GetByIdAsync(id.ToString()).Result;

                if (doc == null)
                    return null;

                dto = JsonConvert.DeserializeObject<BookOrderLineConflictDto>(doc);
            }

            return BookOrderLinePriceConflict.CreateExisting(
                Guid.Parse(dto._id), dto.BookOrderId, dto.BookOrderLineId,
                99M, false, DateTime.Now);
        }

        public IEnumerable<BookOrderLineConflict> Get()
        {
            throw new NotImplementedException();
        }
    }
}