using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GalileoAgentNet.WebApi;

namespace Sample.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(configuration =>
            {
                AreaRegistration.RegisterAllAreas();
                WebApiConfig.Register(configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                
                configuration.MessageHandlers.Add(new GalileoAgentDelegatingHandler("SERVICE_TOKEN"));
            });
        }
    }
}
