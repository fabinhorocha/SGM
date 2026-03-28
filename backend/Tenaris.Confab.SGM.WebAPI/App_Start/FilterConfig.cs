using System.Web;
using System.Web.Mvc;
using Tenaris.Confab.SGM.WebAPI.Filter;

namespace Tenaris.Confab.SGM.WebAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            
            filters.Add(new HandleErrorAttribute());

        }
    }
}
