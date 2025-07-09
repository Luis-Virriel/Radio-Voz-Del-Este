using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    [Authorize]
    public class RadioProgramsController : BaseController
    {
        private RadioEntities db = new RadioEntities();

        // GET: RadioPrograms
        public ActionResult Index()
        {
            var permisos = Session["Permissions"] as List<string>;

            if (permisos == null || permisos.Count == 0)
            {
                return new HttpUnauthorizedResult("No tiene permisos para acceder a esta sección.");
            }

            try
            {
                var radioPrograms = db.RadioPrograms.ToList();

                ViewBag.Permissions = permisos;

                System.Diagnostics.Debug.WriteLine("Permisos cargados: " + string.Join(", ", permisos));

                return View(radioPrograms);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en Index RadioPrograms: " + ex.Message);
                ViewBag.ErrorMessage = "Error al cargar la lista de programas de radio.";
                ViewBag.Permissions = permisos ?? new List<string>();
                return View(new List<RadioProgram>());
            }
        }

        // GET: RadioPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (!HasPermission("ViewProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para ver detalles.");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var radioProgram = db.RadioPrograms.Find(id);
            if (radioProgram == null)
                return HttpNotFound();

            return View(radioProgram);
        }

        // GET: RadioPrograms/Create
        public ActionResult Create()
        {
            if (!HasPermission("CreateProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para crear programas.");

            return View();
        }

        // POST: RadioPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProgramName,RadioDescription,Schedule")] RadioProgram radioProgram, HttpPostedFileBase ImageFile)
        {
            if (!HasPermission("CreateProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para crear programas.");

            try
            {
                if (ModelState.IsValid)
                {
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var ext = Path.GetExtension(ImageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(ext))
                        {
                            ModelState.AddModelError("ImageFile", "Solo se permiten imágenes JPG, PNG, GIF.");
                            return View(radioProgram);
                        }

                        if (ImageFile.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageFile", "El archivo no puede superar los 5MB.");
                            return View(radioProgram);
                        }

                        var fileName = Guid.NewGuid() + ext;
                        var uploadPath = Server.MapPath("~/Content/Images/");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var path = Path.Combine(uploadPath, fileName);
                        ImageFile.SaveAs(path);

                        radioProgram.ImageURL = "/Content/Images/" + fileName;
                    }
                    else
                    {
                        radioProgram.ImageURL = "/Content/Images/default-program.png";
                    }

                    db.RadioPrograms.Add(radioProgram);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
            }

            return View(radioProgram);
        }

        // GET: RadioPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!HasPermission("EditProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para editar programas.");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var radioProgram = db.RadioPrograms.Find(id);
            if (radioProgram == null)
                return HttpNotFound();

            return View(radioProgram);
        }

        // POST: RadioPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProgramName,RadioDescription,Schedule,ImageURL")] RadioProgram radioProgram, HttpPostedFileBase ImageFile)
        {
            if (!HasPermission("EditProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para editar programas.");

            try
            {
                if (ModelState.IsValid)
                {
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var ext = Path.GetExtension(ImageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(ext))
                        {
                            ModelState.AddModelError("ImageFile", "Solo se permiten imágenes JPG, PNG, GIF.");
                            return View(radioProgram);
                        }

                        if (ImageFile.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageFile", "El archivo no puede superar los 5MB.");
                            return View(radioProgram);
                        }

                        var old = db.RadioPrograms.AsNoTracking().FirstOrDefault(r => r.ID == radioProgram.ID);
                        if (old != null && !string.IsNullOrEmpty(old.ImageURL) && !old.ImageURL.Contains("default-program.png"))
                        {
                            var oldPath = Server.MapPath("~" + old.ImageURL);
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        var fileName = Guid.NewGuid() + ext;
                        var uploadPath = Server.MapPath("~/Content/Images/");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        var path = Path.Combine(uploadPath, fileName);
                        ImageFile.SaveAs(path);
                        radioProgram.ImageURL = "/Content/Images/" + fileName;
                    }
                    else
                    {
                        var current = db.RadioPrograms.AsNoTracking().FirstOrDefault(r => r.ID == radioProgram.ID);
                        if (current != null)
                            radioProgram.ImageURL = current.ImageURL;
                    }

                    db.Entry(radioProgram).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
            }

            return View(radioProgram);
        }

        // GET: RadioPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!HasPermission("DeleteProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para eliminar programas.");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var radioProgram = db.RadioPrograms.Find(id);
            if (radioProgram == null)
                return HttpNotFound();

            return View(radioProgram);
        }

        // POST: RadioPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!HasPermission("DeleteProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para eliminar programas.");

            var radioProgram = db.RadioPrograms.Find(id);

            if (radioProgram != null)
            {
                if (!string.IsNullOrEmpty(radioProgram.ImageURL) && !radioProgram.ImageURL.Contains("default-program.png"))
                {
                    var imgPath = Server.MapPath("~" + radioProgram.ImageURL);
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }

                db.RadioPrograms.Remove(radioProgram);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
