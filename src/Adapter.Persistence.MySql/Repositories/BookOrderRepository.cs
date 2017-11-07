using System;
using System.Collections.Generic;
using System.Data;
using Adapter.Persistence.MySql.Repositories.Actions;
using Domain.Entities;
using Domain.Ports.Persistence;
using MySql.Data.MySqlClient;

namespace Adapter.Persistence.MySql.Repositories
{
    internal class BookOrderRepository : IBookOrderRepository
    {
        private readonly string _connectionString;

        public BookOrderRepository(string connectionString)
        {            
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public BookOrder Get(Guid id)
        {
            using (var connection = CreateConnection())
            {
                return GetBookOrderByIdAction.Execute(connection, id);
            }
        }       

        public void Store(BookOrder bookOrder)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    bool bookOrderExists = BookOrderExistsAction.Execute(connection, bookOrder);

                    if (bookOrderExists)
                        UpdateBookOrderAction.Execute(connection, bookOrder);
                    else
                        InsertBookOrderAction.Execute(connection, bookOrder);

                    transaction.Commit();
                }
            }
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier)
        {
            using (var connection = CreateConnection())
            {
                return GetBookOrdersAction.Execute(connection, supplier);
            }
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier, BookOrderState state)
        {
            using (var connection = CreateConnection())
            {
                return GetBookOrdersAction.Execute(connection, supplier, state);
            }            
        }

        public IEnumerable<BookOrder> GetByState(BookOrderState state)
        {
            using (var connection = CreateConnection())
            {
                return GetBookOrdersAction.Execute(connection, state: state);
            }            
        }

        public IEnumerable<BookOrder> Get()
        {
            using (var connection = CreateConnection())
            {
                return GetBookOrdersAction.Execute(connection);
            }
        }

        public void Delete()
        {
            using (var connection = CreateConnection())
            {
                DeleteBookOrdersAction.Execute(connection);
            }
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);                        
        }
    }
}