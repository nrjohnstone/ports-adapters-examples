using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.XPath;
using Adapter.Persistence.MySql.Repositories.Actions;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Adapter.Persistence.MySql.Repositories.Mappers;
using Domain.Entities;
using Domain.Ports.Persistence;
using MySql.Data.MySqlClient;

namespace Adapter.Persistence.MySql.Repositories
{
    internal class BookOrderLineConflictRepository : IBookOrderLineConflictRepository
    {
        private readonly string _connectionString;

        public BookOrderLineConflictRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public void Store(BookOrderLineConflict bookOrderLineConflict)
        {
            var dto = bookOrderLineConflict.ToDto();

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    InsertBookOrderLineConflictAction.Execute(connection, dto);
                    transaction.Commit();
                }
            }
        }

        public void Store(IEnumerable<BookOrderLineConflict> conflicts)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var bookOrderLineConflict in conflicts)
                    {
                        var dto = bookOrderLineConflict.ToDto();
                        InsertBookOrderLineConflictAction.Execute(connection, dto);
                    }
                    transaction.Commit();
                }
            }
        }

        public BookOrderLineConflict Get(Guid id)
        {
            BookOrderLineConflictDto dto = null;

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    dto = GetBookOrderLineConflictsByIdAction.Execute(connection, id);
                    transaction.Commit();
                }
            }

            return dto?.ToEntity();
        }

        public IEnumerable<BookOrderLineConflict> Get()
        {
            IEnumerable<BookOrderLineConflictDto> results = null;

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    results = GetBookOrderLineConflictsAction.Execute(connection);
                    transaction.Commit();
                }
            }

            var bookOrderLineConflicts = results?.Select(dto => dto.ToEntity());

            return bookOrderLineConflicts;
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}