using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace HostApp.WebService.Client1.Tests.Unit
{
    public static class HttpClientExtensions
    {
        public static HttpResponseMessage Put(this HttpClient client, string uri, object body)
        {
            var stringContent = CreateStringContent(body);
            return client.PutAsync(uri, stringContent).Result;
        }

        private static StringContent CreateStringContent(object body)
        {
            var content = JsonConvert.SerializeObject(body);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return stringContent;
        }

        public static HttpResponseMessage Post(this HttpClient client, string uri, object body)
        {
            var stringContent = CreateStringContent(body);
            return client.PostAsync(uri, stringContent).Result;
        }
    }
}