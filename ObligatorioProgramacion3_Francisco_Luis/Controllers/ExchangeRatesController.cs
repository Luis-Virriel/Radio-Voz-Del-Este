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
    public class ExchangeRatesController : Controller
    {
        private RadioEntities db = new RadioEntities();

        public ActionResult Index()
        {
            return View(db.ExchangeRates.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExchangeRate exchangeRate = db.ExchangeRates.Find(id);
            if (exchangeRate == null)
            {
                return HttpNotFound();
            }
            return View(exchangeRate);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ExchangeDate,CurrencyType,ExchangeValue")] ExchangeRate exchangeRate)
        {
            if (ModelState.IsValid)
            {
                db.ExchangeRates.Add(exchangeRate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(exchangeRate);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExchangeRate exchangeRate = db.ExchangeRates.Find(id);
            if (exchangeRate == null)
            {
                return HttpNotFound();
            }
            return View(exchangeRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ExchangeDate,CurrencyType,ExchangeValue")] ExchangeRate exchangeRate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exchangeRate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(exchangeRate);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExchangeRate exchangeRate = db.ExchangeRates.Find(id);
            if (exchangeRate == null)
            {
                return HttpNotFound();
            }
            return View(exchangeRate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExchangeRate exchangeRate = db.ExchangeRates.Find(id);
            db.ExchangeRates.Remove(exchangeRate);
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
