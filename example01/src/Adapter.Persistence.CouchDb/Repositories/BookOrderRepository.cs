using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Adapter.Persistence.CouchDb.Repositories.Dtos;
using Adapter.Persistence.CouchDb.Views;
using Domain.Entities;
using Domain.Ports.Persistence;
using MyCouch;
using MyCouch.Requests;
using MyCouch.Responses;
using Newtonsoft.Json;

namespace Adapter.Persistence.CouchDb.Repositories
{
    public class BookOrderRepository : IBookOrderRepository
    {
        private readonly string _databaseUri;
        private readonly string _databaseName;

        public BookOrderRepository(CouchDbSettings settings)
        {
            _databaseUri = settings.DatabaseUri;
            _databaseName = settings.DatabaseName;
        }

        public BookOrder Get(Guid id)
        {
            BookOrderDto retrieved;

            using (var store = new MyCouchStore(_databaseUri, _databaseName))
            {
                var doc = store.GetByIdAsync(id.ToString()).Result;

                if (doc == null)
                    return null;

                retrieved = JsonConvert.DeserializeObject<BookOrderDto>(doc);
            }

            return BookOrderMapper.MapFrom(retrieved);
        }

        public void Store(BookOrder bookOrder)
        {
            using (var store = new MyCouchStore(_databaseUri, _databaseName))
            {
                // HACK: Concurrency support, lets just get the latest rev
                // Don't do this in production environments !! Implement concurrency properly all the way
                // to the client !!
                string rev = GetRev(store, bookOrder.Id);

                BookOrderDto bookOrderDto = BookOrderMapper.MapTo(bookOrder, rev);

                if (rev == null)
                {
                    store.StoreAsync(bookOrderDto).Wait();
                }
                else
                {
                    bookOrderDto._rev = rev;
                    store.StoreAsync(bookOrderDto).Wait();
                }
            }
        }

        private string GetRev(MyCouchStore store, Guid id)
        {
            var doc = store.GetByIdAsync(id.ToString()).Result;

            if (doc == null)
                return null;

            var retrieved = store.GetByIdAsync<BookOrderDto>(id.ToString()).Result;
            return retrieved._rev;
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier)
        {
            List<BookOrder> bookOrders = new List<BookOrder>();
            using (var client = new MyCouchClient(_databaseUri, _databaseName))
            {
                QueryViewRequest request = new QueryViewRequest(
                    "bookorders", "bysupplier");
                request.Configure(parameters => parameters
                    .Key(supplier)
                    .IncludeDocs(true));

                ViewQueryResponse<string> results =
                    client.Views.QueryAsync<string>(request).Result;

                foreach (var resultsRow in results.Rows)
                {
                    BookOrderDto dto = JsonConvert.DeserializeObject<BookOrderDto>(
                        resultsRow.IncludedDoc);
                    bookOrders.Add(BookOrderMapper.MapFrom(dto));
                }

            }
            return bookOrders;
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier, BookOrderState state)
        {
            List<BookOrder> bookOrders = new List<BookOrder>();
            using (var client = new MyCouchClient(_databaseUri, _databaseName))
            {
                QueryViewRequest request = new QueryViewRequest(
                    "bookorders", "bysupplier");
                request.Configure(parameters => parameters
                    .Key(supplier)
                    .IncludeDocs(false));

                ViewQueryResponse<string> docIdsBySupplier =
                    client.Views.QueryAsync<string>(request).Result;

                if (docIdsBySupplier.IsEmpty)
                    return Enumerable.Empty<BookOrder>();

                QueryViewRequest request2 = new QueryViewRequest(
                    "bookorders", "bystate");
                request2.Configure(parameters => parameters
                    .Key(state)
                    .IncludeDocs(false));

                ViewQueryResponse<string> docIdsByState =
                    client.Views.QueryAsync<string>(request2).Result;

                if (docIdsByState.IsEmpty)
                    return Enumerable.Empty<BookOrder>();

                var docIds = docIdsBySupplier.Rows.Select(x => x.Id)
                    .Intersect(docIdsByState.Rows.Select(x => x.Id));

                var request3 = new QueryViewRequest("_all_docs");
                request3.Configure(parameters => parameters
                    .Keys(docIds.ToArray())
                    .IncludeDocs(true));

                var results =
                    client.Views.QueryAsync<string>(request3).Result;

                if (results.IsEmpty)
                    return Enumerable.Empty<BookOrder>();

                foreach (var resultsRow in results.Rows)
                {
                    BookOrderDto dto = JsonConvert.DeserializeObject<BookOrderDto>(
                        resultsRow.IncludedDoc);
                    bookOrders.Add(BookOrderMapper.MapFrom(dto));
                }
            }
            return bookOrders;
        }

        public IEnumerable<BookOrder> GetByState(BookOrderState state)
        {
            List<BookOrder> bookOrders = new List<BookOrder>();
            using (var client = new MyCouchClient(_databaseUri, _databaseName))
            {
                QueryViewRequest request = new QueryViewRequest(
                    "bookorders", "bystate");
                request.Configure(parameters => parameters
                    .Key(state)
                    .IncludeDocs(true));

                ViewQueryResponse<string> results =
                    client.Views.QueryAsync<string>(request).Result;

                foreach (var resultsRow in results.Rows)
                {
                    BookOrderDto dto = JsonConvert.DeserializeObject<BookOrderDto>(
                        resultsRow.IncludedDoc);
                    bookOrders.Add(BookOrderMapper.MapFrom(dto));
                }
            }
            return bookOrders;
        }

        public IEnumerable<BookOrder> Get()
        {
            List<BookOrder> bookOrders = new List<BookOrder>();
            using (var client = new MyCouchClient(_databaseUri, _databaseName))
            {
                QueryViewRequest request = new QueryViewRequest(
                    "bookorders", "allOrders");
                request.Configure(parameters => parameters.IncludeDocs(true));

                ViewQueryResponse<string> results =
                    client.Views.QueryAsync<string>(request).Result;

                foreach (var resultsRow in results.Rows)
                {
                    BookOrderDto dto = JsonConvert.DeserializeObject<BookOrderDto>(
                        resultsRow.IncludedDoc);
                    bookOrders.Add(BookOrderMapper.MapFrom(dto));
                }
            }
            return bookOrders;
        }

        public void Delete()
        {
            using (var client = new MyCouchClient(_databaseUri, _databaseName))
            {
                QueryViewRequest request = new QueryViewRequest(
                    "bookorders", "allOrders");
                request.Configure(parameters => parameters.IncludeDocs(true));

                ViewQueryResponse<string> results =
                    client.Views.QueryAsync<string>(request).Result;

                var bulkRequest = new BulkRequest() { AllOrNothing = false };
                foreach (var resultsRow in results.Rows)
                {
                    BookOrderDto dto = JsonConvert.DeserializeObject<BookOrderDto>(
                        resultsRow.IncludedDoc);
                    bulkRequest.Delete(dto._id, dto._rev);
                    Console.WriteLine($"Delete request for id:{dto._id}");
                }

                client.Documents.BulkAsync(bulkRequest).Wait();
            }
        }
    }
}