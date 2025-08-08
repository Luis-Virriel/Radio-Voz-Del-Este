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

                return View(radioPrograms);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar la lista de programas de radio.";
                ViewBag.Permissions = permisos ?? new List<string>();
                return View(new List<RadioProgram>());
            }
        }

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

        public ActionResult Create()
        {
            if (!HasPermission("CreateProgram"))
                return new HttpUnauthorizedResult("No tiene permiso para crear programas.");

            return View();
        }

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
                    // Verificar si ya existe un programa en el mismo horario
                    bool horarioOcupado = db.RadioPrograms.Any(rp => rp.Schedule == radioProgram.Schedule);
                    if (horarioOcupado)
                    {
                        ModelState.AddModelError("Schedule", "Ya existe un programa en ese horario.");
                        return View(radioProgram);
                    }

                    // Validar nombre único
                    bool nombreRepetido = db.RadioPrograms.Any(rp => rp.ProgramName == radioProgram.ProgramName);
                    if (nombreRepetido)
                    {
                        ModelState.AddModelError("ProgramName", "Ya existe un programa de radio con ese nombre.");
                        return View(radioProgram);
                    }

                    // Procesar imagen
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
                    // Verificar que no exista otro programa en el mismo horario (excepto el actual)
                    bool horarioOcupado = db.RadioPrograms.Any(rp => rp.Schedule == radioProgram.Schedule && rp.ID != radioProgram.ID);
                    if (horarioOcupado)
                    {
                        ModelState.AddModelError("Schedule", "Ya existe otro programa en ese horario.");
                        return View(radioProgram);
                    }

                    // Validar nombre único (excepto el actual)
                    bool nombreRepetido = db.RadioPrograms.Any(rp => rp.ProgramName == radioProgram.ProgramName && rp.ID != radioProgram.ID);
                    if (nombreRepetido)
                    {
                        ModelState.AddModelError("ProgramName", "Ya existe otro programa de radio con ese nombre.");
                        return View(radioProgram);
                    }

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
