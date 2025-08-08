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
    public class WeathersController : Controller
    {
        private RadioEntities db = new RadioEntities();

        public ActionResult Index()
        {
            return View(db.Weathers.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weather weather = db.Weathers.Find(id);
            if (weather == null)
            {
                return HttpNotFound();
            }
            return View(weather);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,WeatherDate,Temperature,WeatherDescription,Icon")] Weather weather)
        {
            if (ModelState.IsValid)
            {
                db.Weathers.Add(weather);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(weather);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weather weather = db.Weathers.Find(id);
            if (weather == null)
            {
                return HttpNotFound();
            }
            return View(weather);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,WeatherDate,Temperature,WeatherDescription,Icon")] Weather weather)
        {
            if (ModelState.IsValid)
            {
                db.Entry(weather).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(weather);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weather weather = db.Weathers.Find(id);
            if (weather == null)
            {
                return HttpNotFound();
            }
            return View(weather);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Weather weather = db.Weathers.Find(id);
            db.Weathers.Remove(weather);
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
