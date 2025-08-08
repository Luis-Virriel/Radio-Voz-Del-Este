using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class UsersController : BaseController
    {
        private RadioEntities db = new RadioEntities();

        public ActionResult Index()
        {
            if (Session["Permissions"] == null || ((System.Collections.Generic.List<string>)Session["Permissions"]).Count == 0)
                return RedirectToAction("Login", "Account");

            var users = db.Users.Include(u => u.Role);
            return View(users.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (!HasPermission("ViewUser"))
                return RedirectToAction("Login", "Account");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        public ActionResult Create()
        {
            if (!HasPermission("CreateUser"))
                return RedirectToAction("Login", "Account");

            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserName,Email,UserPassword,RoleID")] User user)
        {
            if (!HasPermission("CreateUser"))
                return RedirectToAction("Login", "Account");

            if (db.Users.Any(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "El nombre de usuario ya existe.");
            }

            if (db.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                user.UserPassword = HashPasswordBCrypt(user.UserPassword);
                db.Users.Add(user);
                db.SaveChanges();

                if (user.RoleID == 3)
                {
                    return RedirectToAction("Create", "Clients", new { userId = user.ID });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName", user.RoleID);
            return View(user);
        }


        public ActionResult Edit(int? id)
        {
            if (!HasPermission("EditUser"))
                return RedirectToAction("Login", "Account");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            user.UserPassword = null;

            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName", user.RoleID);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,Email,UserPassword,RoleID")] User user)
        {
            if (!HasPermission("EditUser"))
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var userInDb = db.Users.AsNoTracking().FirstOrDefault(u => u.ID == user.ID);
                if (userInDb == null)
                    return HttpNotFound();

                if (!string.IsNullOrWhiteSpace(user.UserPassword))
                {
                    user.UserPassword = HashPasswordBCrypt(user.UserPassword);
                }
                if (db.Users.Any(u => u.UserName == user.UserName))
                {
                    ModelState.AddModelError("UserName", "El nombre de usuario ya existe.");
                }
                else 
                {
                    user.UserPassword = userInDb.UserPassword;
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName", user.RoleID);
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (!HasPermission("DeleteUser"))
                return RedirectToAction("Login", "Account");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!HasPermission("DeleteUser"))
                return RedirectToAction("Login", "Account");

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            var clienteVinculado = db.Clients.Any(c => c.UserID == id);
            if (clienteVinculado)
            {
                ModelState.AddModelError("", "No se puede eliminar el usuario porque tiene un cliente vinculado.");
                return View(user);
            }

            try
            {
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al eliminar usuario: " + ex.Message);
                ModelState.AddModelError("", "Ocurrió un error al eliminar el usuario.");
                return View(user);
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        private string HashPasswordBCrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
