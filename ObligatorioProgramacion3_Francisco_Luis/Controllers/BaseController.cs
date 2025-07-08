using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class BaseController : Controller
    {
        private RadioEntities db = new RadioEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Solo si está logueado:
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;

                // Trae User con Role y Permissions:
                var user = db.Users
                             .Include(u => u.Role.Permissions)
                             .FirstOrDefault(u => u.UserName == userName);

                if (user != null)
                {
                    Session["Role"] = user.Role.RoleName;
                    Session["Permissions"] = user.Role.Permissions.Select(p => p.PermissionName).ToList();
                }
            }
            else
            {
                Session["Role"] = null;
                Session["Permissions"] = null;
            }
        }
    }
}
