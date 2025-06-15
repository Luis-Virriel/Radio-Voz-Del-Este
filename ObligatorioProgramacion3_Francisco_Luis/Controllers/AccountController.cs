using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using ObligatorioProgramacion3_Francisco_Luis.Models;

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

            string hashedPassword = HashPasswordSHA256(password);

            var user = db.Users
                         .FirstOrDefault(u =>
                             u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                             u.UserPassword == hashedPassword);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                Session["Role"] = user.Role.RoleName;
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

        // Helper: genera hash SHA256
        private string HashPasswordSHA256(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }
    }
}
