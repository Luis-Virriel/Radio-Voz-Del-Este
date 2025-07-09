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
    public class ClientsController : BaseController
    {
        private RadioEntities db = new RadioEntities();

        // GET: Clients
        public ActionResult Index()
        {
            var permisos = Session["Permissions"] as List<string>;
            if (permisos == null || permisos.Count == 0)
            {
                return new HttpUnauthorizedResult("No tiene permisos para acceder a esta sección.");
            }

            try
            {
                var clients = db.Clients.Include(c => c.User);
                return View(clients.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en Index: " + ex.Message);
                ViewBag.ErrorMessage = "Error al cargar la lista de clientes.";
                return View(new List<Client>());
            }
        }

        // GET: Clients/Details/5
        public ActionResult Details(string id)
        {
            if (!HasPermission("View"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para ver detalles de clientes.");
            }

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Client client = db.Clients.Find(id);
                if (client == null)
                {
                    return HttpNotFound();
                }
                return View(client);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en Details: " + ex.Message);
                return HttpNotFound();
            }
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            if (!HasPermission("CreateClient"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para crear clientes.");
            }

            try
            {
                ViewBag.UserID = new SelectList(db.Users, "ID", "UserName");
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar Create: " + ex.Message);
                ViewBag.ErrorMessage = "Error al cargar el formulario de creación.";
                return View();
            }
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDNumber,FirstName,LastName,Email,BirthDate,UserID")] Client client)
        {
            if (!HasPermission("CreateClient"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para crear clientes.");
            }

            try
            {
                // Validaciones únicas
                if (!string.IsNullOrEmpty(client.IDNumber))
                {
                    var existingClient = db.Clients.Find(client.IDNumber);
                    if (existingClient != null)
                    {
                        ModelState.AddModelError("IDNumber", "Ya existe un cliente con este número de identificación.");
                    }
                }

                if (!string.IsNullOrEmpty(client.Email))
                {
                    var existingEmail = db.Clients.FirstOrDefault(c => c.Email == client.Email);
                    if (existingEmail != null)
                    {
                        ModelState.AddModelError("Email", "Ya existe un cliente con este correo electrónico.");
                    }
                }

                if (client.UserID.HasValue)
                {
                    var existingUser = db.Clients.FirstOrDefault(c => c.UserID == client.UserID);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("UserID", "El usuario seleccionado ya está asociado a otro cliente.");
                    }
                }

                if (ModelState.IsValid)
                {
                    db.Clients.Add(client);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cliente creado exitosamente.";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entidad con error: {0}", eve.Entry.Entity.GetType().Name);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Propiedad: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
                        ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error de actualización de BD: " + ex.Message);
                ModelState.AddModelError("", "Error al guardar en la base de datos. Verifique que los datos sean correctos.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error general en Create: " + ex.Message);
                ModelState.AddModelError("", "Ocurrió un error inesperado al guardar el cliente.");
            }

            // Recargar ViewBag para el dropdown de usuarios
            try
            {
                ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", client.UserID);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al recargar Users: " + ex.Message);
                ViewBag.UserID = new SelectList(new List<SelectListItem>());
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(string id)
        {
            if (!HasPermission("EditClient"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para editar clientes.");
            }

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Client client = db.Clients.Find(id);
                if (client == null)
                {
                    return HttpNotFound();
                }
                ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", client.UserID);
                return View(client);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en Edit GET: " + ex.Message);
                return HttpNotFound();
            }
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDNumber,FirstName,LastName,Email,BirthDate,UserID")] Client client)
        {
            if (!HasPermission("EditClient"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para editar clientes.");
            }

            try
            {
                if (!string.IsNullOrEmpty(client.Email))
                {
                    var existingEmail = db.Clients.FirstOrDefault(c => c.Email == client.Email && c.IDNumber != client.IDNumber);
                    if (existingEmail != null)
                    {
                        ModelState.AddModelError("Email", "Ya existe otro cliente con este correo electrónico.");
                    }
                }

                if (client.UserID.HasValue)
                {
                    var existingUser = db.Clients.FirstOrDefault(c => c.UserID == client.UserID && c.IDNumber != client.IDNumber);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("UserID", "El usuario seleccionado ya está asociado a otro cliente.");
                    }
                }

                if (ModelState.IsValid)
                {
                    db.Entry(client).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
                    return RedirectToAction("Index");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entidad con error: {0}", eve.Entry.Entity.GetType().Name);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Propiedad: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
                        ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error de actualización de BD: " + ex.Message);
                ModelState.AddModelError("", "Error al actualizar en la base de datos.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error general en Edit: " + ex.Message);
                ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar el cliente.");
            }

            try
            {
                ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", client.UserID);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al recargar Users en Edit: " + ex.Message);
                ViewBag.UserID = new SelectList(new List<SelectListItem>());
            }

            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(string id)
        {
            if (!HasPermission("DeleteClient"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para eliminar clientes.");
            }

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Client client = db.Clients.Find(id);
                if (client == null)
                {
                    return HttpNotFound();
                }
                return View(client);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en Delete GET: " + ex.Message);
                return HttpNotFound();
            }
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!HasPermission("DeleteClient"))
            {
                return new HttpUnauthorizedResult("No tiene permiso para eliminar clientes.");
            }

            try
            {
                Client client = db.Clients.Find(id);
                if (client == null)
                {
                    return HttpNotFound();
                }

                db.Clients.Remove(client);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cliente eliminado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al eliminar cliente: " + ex.Message);
                TempData["ErrorMessage"] = "No se puede eliminar el cliente porque tiene registros relacionados.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error general en Delete: " + ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el cliente.";
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
