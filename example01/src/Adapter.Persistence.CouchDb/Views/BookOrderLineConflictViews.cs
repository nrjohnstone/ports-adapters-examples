using Newtonsoft.Json;

namespace Adapter.Persistence.CouchDb.Views
{
    public class BookOrderLineConflictViews : ICouchDbView
    {
        public string Id => "_design/" + DesignDocument;
        public string DesignDocument => "bookorderLineConflicts";

        public string Json()
        {
            return JsonConvert.SerializeObject(new
            {
                _id = Id,
                language = "javascript",
                views = new
                {
                    all = new
                    {
                        map = "function(doc) { " +
                              "if(doc.$doctype !== 'bookOrderLineConflictDto' || doc._deleted) return;" +
                              "emit(doc.id, 0);" +
                              "}"
                    }
                }
            });
        }
    }
}