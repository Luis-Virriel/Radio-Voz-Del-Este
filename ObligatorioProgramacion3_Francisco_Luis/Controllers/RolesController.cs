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

        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var role = db.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.ID == id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

        public ActionResult Create()
        {
            return View();
        }

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

                role.Permissions.Clear();

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

            var allPermissions = db.Permissions.ToList();
            model.Permissions = allPermissions.Select(p => new PermissionCheckbox
            {
                PermissionID = p.ID,
                PermissionName = p.PermissionName,
                IsAssigned = model.Permissions.Any(mp => mp.PermissionID == p.ID && mp.IsAssigned)
            }).ToList();

            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Role role = db.Roles.Find(id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

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
