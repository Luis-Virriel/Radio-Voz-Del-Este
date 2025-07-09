using ObligatorioProgramacion3_Francisco_Luis.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class AccountController : Controller
    {
        private RadioEntities db = new RadioEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Include("Role.Permissions").FirstOrDefault(u => u.UserName == username);
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.UserPassword))
                {
                    // Emitir cookie de autenticación
                    FormsAuthentication.SetAuthCookie(user.UserName, false);

                    // Guardar rol y permisos en sesión
                    Session["Role"] = user.Role.RoleName;
                    Session["Permissions"] = user.Role.Permissions.Select(p => p.PermissionName).ToList();

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            }

            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
