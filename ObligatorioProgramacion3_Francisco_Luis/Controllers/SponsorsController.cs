using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    [Authorize]
    public class SponsorsController : BaseController
    {
        private RadioEntities db = new RadioEntities();

        public ActionResult Index()
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            bool tienePermisoSponsor = permisos.Any(p =>
                p.Contains("Sponsor")
            );

            if (!tienePermisoSponsor)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(db.Sponsors.ToList());
        }


        public ActionResult Details(int? id)
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            if (!permisos.Contains("ViewSponsor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsors.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        public ActionResult Create()
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            if (!permisos.Contains("CreateSponsor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SponsorsName,SponsorDescription,CantPlan")] Sponsor sponsor)
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            if (!permisos.Contains("CreateSponsor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                bool existeNombre = db.Sponsors.Any(s => s.SponsorsName == sponsor.SponsorsName);
                if (existeNombre)
                {
                    ModelState.AddModelError("SponsorsName", "Ya existe un patrocinador con ese nombre.");
                    return View(sponsor);
                }

                db.Sponsors.Add(sponsor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sponsor);
        }

        public ActionResult Edit(int? id)
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            if (!permisos.Contains("EditSponsor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsors.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SponsorsName,SponsorDescription,CantPlan")] Sponsor sponsor)
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            if (!permisos.Contains("EditSponsor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                bool existeNombre = db.Sponsors.Any(s => s.SponsorsName == sponsor.SponsorsName && s.ID != sponsor.ID);
                if (existeNombre)
                {
                    ModelState.AddModelError("SponsorsName", "Ya existe otro patrocinador con ese nombre.");
                    return View(sponsor);
                }

                db.Entry(sponsor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sponsor);
        }
        public ActionResult Delete(int? id)
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            if (!permisos.Contains("DeleteSponsor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsors.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var permisos = Session["Permissions"] as List<string> ?? new List<string>();

            if (!permisos.Contains("DeleteSponsor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Sponsor sponsor = db.Sponsors.Find(id);
            db.Sponsors.Remove(sponsor);
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
