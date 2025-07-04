using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class RolesController : BaseController
    {
        private RadioEntities db = new RadioEntities();

        // GET: Roles
        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var role = db.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.ID == id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var role = db.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.ID == id);
            if (role == null)
                return HttpNotFound();

            var allPermissions = db.Permissions.ToList();
            var rolePermissionsIds = role.Permissions.Select(p => p.ID).ToList();

            var permissionCheckboxes = allPermissions.Select(p => new PermissionCheckbox
            {
                PermissionID = p.ID,
                PermissionName = p.PermissionName,
                IsAssigned = rolePermissionsIds.Contains(p.ID)
            }).ToList();

            var model = new EditRoleViewModel
            {
                ID = role.ID,
                RoleName = role.RoleName,
                Permissions = permissionCheckboxes
            };

            return View(model);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = db.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.ID == model.ID);
                if (role == null)
                    return HttpNotFound();

                role.RoleName = model.RoleName;

                // Limpiar permisos actuales
                role.Permissions.Clear();

                // Agregar permisos seleccionados
                var selectedPermissionIds = model.Permissions
                    .Where(p => p.IsAssigned)
                    .Select(p => p.PermissionID)
                    .ToList();

                var selectedPermissions = db.Permissions
                    .Where(p => selectedPermissionIds.Contains(p.ID))
                    .ToList();

                foreach (var permiso in selectedPermissions)
                {
                    role.Permissions.Add(permiso);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Si hay error, recargar permisos para que la vista los muestre bien
            var allPermissions = db.Permissions.ToList();
            model.Permissions = allPermissions.Select(p => new PermissionCheckbox
            {
                PermissionID = p.ID,
                PermissionName = p.PermissionName,
                IsAssigned = model.Permissions.Any(mp => mp.PermissionID == p.ID && mp.IsAssigned)
            }).ToList();

            return View(model);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Role role = db.Roles.Find(id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Role role = db.Roles.Find(id);
            db.Roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
