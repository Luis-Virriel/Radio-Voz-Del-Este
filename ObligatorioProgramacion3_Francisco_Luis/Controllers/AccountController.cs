using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ObligatorioProgramacion3_Francisco_Luis.Models;
using System.Data.Entity;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class AccountController : Controller
    {
        private RadioEntities db = new RadioEntities();

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña";
                return View();
            }

            var user = db.Users
                         .Include(u => u.Role.Permissions)
                         .FirstOrDefault(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.UserPassword))
            {
                FormsAuthentication.SetAuthCookie(user.UserName, false);

                // Guardar en Session
                Session["Role"] = user.Role.RoleName ?? "Sin rol";
                Session["Permissions"] = user.Role.Permissions.Select(p => p.PermissionName).ToList();

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }

        // OPCIONAL: Refrescar permisos sin cerrar sesión
        [Authorize]
        public ActionResult RefreshPermissions()
        {
            var userName = User.Identity.Name;
            var user = db.Users.Include(u => u.Role.Permissions)
                               .FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                Session["Role"] = user.Role.RoleName ?? "Sin rol";
                Session["Permissions"] = user.Role.Permissions.Select(p => p.PermissionName).ToList();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
