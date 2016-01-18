using SAKURA.API.WebApi;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Http.Filters;

namespace SAKURA.API.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(System.Web.Http.Filters.HttpFilterCollection filters)
        {
            //NOTE: Sequence of attaching filter is important
			
			//filters.Add(new SecurityFilterSQLInjection());
			//filters.Add(new SecurityFilterJWT());
        }
    }
}