using System.Web;
using System.Web.Mvc;

namespace ObligatorioProgramacion3_Francisco_Luis
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
