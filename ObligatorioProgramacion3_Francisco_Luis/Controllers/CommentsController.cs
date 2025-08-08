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
    public class CommentsController : Controller
    {
        private RadioEntities db = new RadioEntities();

        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Client).Include(c => c.RadioProgram);
            return View(comments.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "IDNumber", "FirstName");
            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClientID,ProgramID,Comment1,CommentDate")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "IDNumber", "FirstName", comment.ClientID);
            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName", comment.ProgramID);
            return View(comment);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "IDNumber", "FirstName", comment.ClientID);
            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName", comment.ProgramID);
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ClientID,ProgramID,Comment1,CommentDate")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "IDNumber", "FirstName", comment.ClientID);
            ViewBag.ProgramID = new SelectList(db.RadioPrograms, "ID", "ProgramName", comment.ProgramID);
            return View(comment);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            var clienteVinculado = db.Clients.Any(c => c.UserID == id);
            if (clienteVinculado)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el usuario porque tiene un cliente vinculado.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al eliminar usuario: " + ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el usuario.";
            }

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
