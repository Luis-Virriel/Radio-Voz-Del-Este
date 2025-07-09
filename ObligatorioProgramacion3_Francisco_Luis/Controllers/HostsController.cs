using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    [Authorize]
    public class HostsController : BaseController
    {
        private RadioEntities db = new RadioEntities();

        // ✅ GET: Hosts - alineado a ClientsController
        public ActionResult Index()
        {
            var permisos = Session["Permissions"] as List<string>;
            if (permisos == null || permisos.Count == 0)
            {
                return new HttpUnauthorizedResult("No tiene permisos para acceder a esta sección.");
            }

            var hosts = db.Hosts.Include(h => h.RadioProgram);
            return View(hosts.ToList());
        }

        // ✅ GET: Hosts/Details/5
        public ActionResult Details(int? id)
        {
            if (!HasPermission("ViewHost"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para ver detalles de Hosts.");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }

            return View(host);
        }

        // ✅ GET: Hosts/Create
        public ActionResult Create()
        {
            if (!HasPermission("CreateHost"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para crear Hosts.");
            }

            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName");
            return View();
        }

        // ✅ POST: Hosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,HostName,Bio,ProgramID")] Host host)
        {
            if (!HasPermission("CreateHost"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para crear Hosts.");
            }

            if (ModelState.IsValid)
            {
                db.Hosts.Add(host);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName", host.ProgramID);
            return View(host);
        }

        // ✅ GET: Hosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!HasPermission("EditHost"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para editar Hosts.");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName", host.ProgramID);
            return View(host);
        }

        // ✅ POST: Hosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,HostName,Bio,ProgramID")] Host host)
        {
            if (!HasPermission("EditHost"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para editar Hosts.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(host).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName", host.ProgramID);
            return View(host);
        }

        // ✅ GET: Hosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!HasPermission("DeleteHost"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para eliminar Hosts.");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }

            return View(host);
        }

        // ✅ POST: Hosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!HasPermission("DeleteHost"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para eliminar Hosts.");
            }

            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }

            db.Hosts.Remove(host);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
