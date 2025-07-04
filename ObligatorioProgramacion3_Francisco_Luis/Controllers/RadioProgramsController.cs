using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProgramName,RadioDescription,Schedule")] RadioProgram radioProgram, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Procesar la imagen si se subió un archivo
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        // Validar el tipo de archivo
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(ImageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("ImageFile", "Solo se permiten archivos de imagen (JPG, PNG, GIF)");
                            return View(radioProgram);
                        }

                        // Validar el tamaño del archivo (5MB máximo)
                        if (ImageFile.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageFile", "El archivo no puede ser mayor a 5MB");
                            return View(radioProgram);
                        }

                        // Crear nombre único para el archivo
                        var fileName = Guid.NewGuid().ToString() + extension;
                        var uploadPath = Server.MapPath("~/Content/Images/");

                        // Crear el directorio si no existe
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        var filePath = Path.Combine(uploadPath, fileName);

                        // Guardar el archivo
                        ImageFile.SaveAs(filePath);

                        // Establecer la URL de la imagen
                        radioProgram.ImageURL = "/Content/Images/" + fileName;
                    }
                    else
                    {
                        // Si no se subió imagen, usar una imagen por defecto o dejar vacío
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProgramName,RadioDescription,Schedule")] RadioProgram radioProgram, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Si se subió una nueva imagen
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        // Validar el tipo de archivo
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(ImageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("ImageFile", "Solo se permiten archivos de imagen (JPG, PNG, GIF)");
                            return View(radioProgram);
                        }

                        // Validar el tamaño del archivo
                        if (ImageFile.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageFile", "El archivo no puede ser mayor a 5MB");
                            return View(radioProgram);
                        }

                        // Obtener el programa actual para eliminar la imagen anterior
                        var currentProgram = db.RadioPrograms.AsNoTracking().FirstOrDefault(x => x.ID == radioProgram.ID);
                        if (currentProgram != null && !string.IsNullOrEmpty(currentProgram.ImageURL))
                        {
                            var oldImagePath = Server.MapPath("~" + currentProgram.ImageURL);
                            if (System.IO.File.Exists(oldImagePath) && !currentProgram.ImageURL.Contains("default-program.png"))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Guardar nueva imagen
                        var fileName = Guid.NewGuid().ToString() + extension;
                        var uploadPath = Server.MapPath("~/Content/Images/");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        var filePath = Path.Combine(uploadPath, fileName);
                        ImageFile.SaveAs(filePath);
                        radioProgram.ImageURL = "/Content/Images/" + fileName;
                    }
                    else
                    {
                        // Mantener la imagen actual
                        var currentProgram = db.RadioPrograms.AsNoTracking().FirstOrDefault(x => x.ID == radioProgram.ID);
                        if (currentProgram != null)
                        {
                            radioProgram.ImageURL = currentProgram.ImageURL;
                        }
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

            // Eliminar la imagen del servidor si existe
            if (!string.IsNullOrEmpty(radioProgram.ImageURL) && !radioProgram.ImageURL.Contains("default-program.png"))
            {
                var imagePath = Server.MapPath("~" + radioProgram.ImageURL);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

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