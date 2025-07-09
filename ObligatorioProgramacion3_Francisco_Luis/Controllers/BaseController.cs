using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class BaseController : Controller
    {
        protected RadioEntities db = new RadioEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;

                var lastRefresh = Session["PermissionsLastRefresh"] as DateTime?;
                var now = DateTime.UtcNow;

                if (lastRefresh == null || (now - lastRefresh.Value).TotalMinutes > 5)
                {
                    RefreshUserSessionPermissions(userName);
                    Session["PermissionsLastRefresh"] = now;
                }
            }
            else
            {
                Session.Clear();
            }
        }

        protected void RefreshUserSessionPermissions(string userName)
        {
            var user = db.Users.Include(u => u.Role.Permissions)
                              .FirstOrDefault(u => u.UserName == userName);

            if (user != null)
            {
                Session["Role"] = user.Role?.RoleName ?? "Sin rol";
                Session["Permissions"] = user.Role?.Permissions.Select(p => p.PermissionName).ToList() ?? new List<string>();
            }
            else
            {
                Session.Clear();
            }
        }

        protected bool HasPermission(string permissionName)
        {
            var permissions = Session["Permissions"] as List<string>;
            return permissions != null && permissions.Contains(permissionName);
        }

        // Nuevo método para chequear si tiene al menos 1 permiso de una lista
        protected bool HasAnyPermission(IEnumerable<string> permissionsToCheck)
        {
            var permissions = Session["Permissions"] as List<string>;
            if (permissions == null || permissions.Count == 0)
                return false;

            return permissions.Any(p => permissionsToCheck.Contains(p));
        }
    }
}
