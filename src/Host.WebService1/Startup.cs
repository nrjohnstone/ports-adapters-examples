using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;
using Swashbuckle.Application;

namespace Host.WebService1
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.Routes.IgnoreRoute("IgnoreAxdFiles", "{resource}.axd/{*pathInfo}");

            config.EnableSwagger(c => c.SingleApiVersion("v1", "BookOrders"))
                .EnableSwaggerUi();

            config.EnsureInitialized();
            
            appBuilder.UseWebApi(config);
        }
    }
}
