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
    public class NewsController : BaseController // <--- ¡HEREDA BaseController!
    {
        private RadioEntities db = new RadioEntities();

        // GET: News
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (!HasPermission("ViewNews"))
                return new HttpUnauthorizedResult("No tiene permiso para ver noticias.");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            News news = db.News.Find(id);
            if (news == null)
                return HttpNotFound();

            return View(news);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            if (!HasPermission("CreateNews"))
                return new HttpUnauthorizedResult("No tiene permiso para crear noticias.");

            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Content,PublishDate,ImageURL")] News news)
        {
            if (!HasPermission("CreateNews"))
                return new HttpUnauthorizedResult("No tiene permiso para crear noticias.");

            if (ModelState.IsValid)
            {
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!HasPermission("EditNews"))
                return new HttpUnauthorizedResult("No tiene permiso para editar noticias.");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            News news = db.News.Find(id);
            if (news == null)
                return HttpNotFound();

            return View(news);
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Content,PublishDate,ImageURL")] News news)
        {
            if (!HasPermission("EditNews"))
                return new HttpUnauthorizedResult("No tiene permiso para editar noticias.");

            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!HasPermission("DeleteNews"))
                return new HttpUnauthorizedResult("No tiene permiso para eliminar noticias.");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            News news = db.News.Find(id);
            if (news == null)
                return HttpNotFound();

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!HasPermission("DeleteNews"))
                return new HttpUnauthorizedResult("No tiene permiso para eliminar noticias.");

            News news = db.News.Find(id);
            db.News.Remove(news);
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
