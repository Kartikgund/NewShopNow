using System.Web;
using System.Web.Mvc;

namespace NewShopNow2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MyActionFilter());
            filters.Add(new CustomExceptionFilter());
        }
    }
}
