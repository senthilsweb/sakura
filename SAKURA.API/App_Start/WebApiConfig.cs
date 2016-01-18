using System.Web.Http;
using System.Web.Http.Cors;

namespace SAKURA.API.WebApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
            //http://www.asp.net/web-api/overview/security/enabling-cross-origin-requests-in-web-api
            config.EnableCors();

			//var corsAttr = new EnableCorsAttribute("*", "Origin, Content-Type, Accept", "GET, PUT, POST, DELETE, OPTIONS");
			//config.EnableCors(corsAttr);

			// Web API routes
			config.MapHttpAttributeRoutes();
			
			//Default route hence should be at the last
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}