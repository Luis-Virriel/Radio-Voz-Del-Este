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

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Role);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserName,Email,UserPassword,RoleID")] User user)
        {
            if (ModelState.IsValid)
            {
                // Hashear la contraseña antes de guardar con BCrypt
                user.UserPassword = HashPasswordBCrypt(user.UserPassword);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName", user.RoleID);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            // Dejar el campo contraseña vacío para que no muestre la contraseña actual
            user.UserPassword = null;

            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName", user.RoleID);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,Email,UserPassword,RoleID")] User user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = db.Users.AsNoTracking().FirstOrDefault(u => u.ID == user.ID);
                if (userInDb == null)
                    return HttpNotFound();

                if (!string.IsNullOrWhiteSpace(user.UserPassword))
                {
                    // Si el usuario ingresó nueva contraseña, hashearla
                    user.UserPassword = HashPasswordBCrypt(user.UserPassword);
                }
                else
                {
                    // Si el campo está vacío, mantener la contraseña anterior
                    user.UserPassword = userInDb.UserPassword;
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "ID", "RoleName", user.RoleID);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        // Función para hashear contraseña usando BCrypt
        private string HashPasswordBCrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
