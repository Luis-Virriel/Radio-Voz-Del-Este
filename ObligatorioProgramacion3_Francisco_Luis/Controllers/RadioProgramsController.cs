using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class RadioProgramsController : Controller
    {
        private RadioEntities db = new RadioEntities();

        // GET: RadioPrograms
        public ActionResult Index()
        {
            return View(db.RadioPrograms.ToList());
        }

        // GET: RadioPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RadioProgram radioProgram = db.RadioPrograms.Find(id);
            if (radioProgram == null)
            {
                return HttpNotFound();
            }
            return View(radioProgram);
        }

        // GET: RadioPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RadioPrograms/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProgramName,ImageURL,RadioDescription,Schedule")] RadioProgram radioProgram)
        {
            if (ModelState.IsValid)
            {
                db.RadioPrograms.Add(radioProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(radioProgram);
        }

        // GET: RadioPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RadioProgram radioProgram = db.RadioPrograms.Find(id);
            if (radioProgram == null)
            {
                return HttpNotFound();
            }
            return View(radioProgram);
        }

        // POST: RadioPrograms/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProgramName,ImageURL,RadioDescription,Schedule")] RadioProgram radioProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(radioProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(radioProgram);
        }

        // GET: RadioPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RadioProgram radioProgram = db.RadioPrograms.Find(id);
            if (radioProgram == null)
            {
                return HttpNotFound();
            }
            return View(radioProgram);
        }

        // POST: RadioPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RadioProgram radioProgram = db.RadioPrograms.Find(id);
            db.RadioPrograms.Remove(radioProgram);
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
