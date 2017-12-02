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
            BookOrderViews bookOrderViews = new BookOrderViews();

            using (var client = new MyCouchClient(_databaseUri, _databaseName))
            {
                var getResponse = client.Documents.GetAsync(bookOrderViews.Id).Result;

                if (getResponse.IsEmpty)
                {
                    DocumentHeaderResponse postResponse = client.Documents.PostAsync(
                        bookOrderViews.Json()).Result;

                    if (postResponse.StatusCode != HttpStatusCode.Created)
                        throw new Exception("Error creating query view");
                }
                else
                {
                    DocumentHeaderResponse putResponse = client.Documents.PutAsync(
                        bookOrderViews.Id,
                        getResponse.Rev,
                        bookOrderViews.Json()).Result;

                    if (putResponse.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception("Error updating query");
                    }
                }

            }
        }
    }
}