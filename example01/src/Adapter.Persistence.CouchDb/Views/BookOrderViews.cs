using Newtonsoft.Json;

namespace Adapter.Persistence.CouchDb.Views
{
    public class BookOrderViews : ICouchDbView
    {
        public string Id => "_design/" + DesignDocument;
        public string DesignDocument => "bookorders";

        public string Json()
        {
            return JsonConvert.SerializeObject(new
            {
                _id = Id,
                language = "javascript",
                views = new
                {
                    bysupplier = new
                    {
                        map = "function(doc) { " +
                              "if(doc.$doctype !== 'bookOrderDto') return;" +
                              "emit(doc.supplier, 0);" +
                              "}"
                    },
                    bystate = new {
                        map = "function(doc) { " +
                              "if(doc.$doctype !== 'bookOrderDto') return;" +
                              "emit(doc.state, 0);" +
                              "}"
                    },
                    allOrders = new
                    {
                        map = "function(doc) { " +
                              "if(doc.$doctype !== 'bookOrderDto' || doc._deleted) return;" +
                              "emit(doc.id, 0);" +
                              "}"
                    }
                }
            });
        }
    }
}