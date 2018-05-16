using System;
using System.Net;
using MyCouch;
using MyCouch.Responses;

namespace Adapter.Persistence.CouchDb.Views
{
    public class ViewManager
    {
        private readonly string _databaseUri;
        private readonly string _databaseName;

        public ViewManager(string databaseUri, string databaseName)
        {
            _databaseUri = databaseUri;
            _databaseName = databaseName;
        }

        public void CreateViews()
        {
            CreateView<BookOrderViews>();
            CreateView<BookOrderLineConflictViews>();
        }

        private void CreateView<T>() where T : ICouchDbView, new()
        {
            T view = new T();

            using (var client = new MyCouchClient(_databaseUri, _databaseName))
            {
                var getResponse = client.Documents.GetAsync(view.Id).Result;

                if (getResponse.IsEmpty)
                {
                    DocumentHeaderResponse postResponse = client.Documents.PostAsync(
                        view.Json()).Result;

                    if (postResponse.StatusCode != HttpStatusCode.Created)
                        throw new Exception("Error creating query view");
                }
                else
                {
                    DocumentHeaderResponse putResponse = client.Documents.PutAsync(
                        view.Id,
                        getResponse.Rev,
                        view.Json()).Result;

                    if (putResponse.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception("Error updating query");
                    }
                }
            }
        }
    }
}