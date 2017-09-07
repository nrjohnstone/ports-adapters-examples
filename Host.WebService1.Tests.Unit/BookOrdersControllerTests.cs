using System.Net;
using FluentAssertions;
using Microsoft.Owin;
using Xunit;

namespace Host.WebService1.Tests.Unit
{
    public class BookOrdersControllerTests : WebServiceTestFixtureBase
    {
        [Fact]
        public void HealthCheck_ShouldReturnOk()
        {
            StartServer();
            var result = Client.GetAsync("health/instance").Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}