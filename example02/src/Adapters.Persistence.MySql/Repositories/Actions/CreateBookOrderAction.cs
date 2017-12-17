using System;
using System.Data;
using Dapper;

namespace Adapters.Persistence.MySql.Repositories.Actions
{
    internal class CreateBookOrderAction
    {
        private readonly IDbConnection _connection;

        public CreateBookOrderAction(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Execute(Guid orderId, string supplier)
        {
            var parameters = new DynamicParameters();

            parameters.Add("OrderId", orderId.ToString());
            parameters.Add("Supplier", supplier);
            parameters.Add("State", "New");

            _connection.Execute(sql: "insert into book_orders (" +
                                     "order_id, " +
                                     "supplier, " +
                                     "state )" +                                
                                     " VALUES " +
                                     "( ?OrderId, " +
                                     " ?Supplier, " +
                                     " ?State )", 
                param: parameters);
        }
    }
}