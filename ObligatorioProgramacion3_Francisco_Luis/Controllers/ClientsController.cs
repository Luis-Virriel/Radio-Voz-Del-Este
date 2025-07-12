using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ObligatorioProgramacion3_Francisco_Luis.Models;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{

    public class ClientsController : BaseController
    {
        private RadioEntities db = new RadioEntities();

        // GET: Clients
        public ActionResult Index()
        {
            if (!HasPermission("ViewClient"))
                return new HttpUnauthorizedResult("No tiene permisos para acceder a esta sección.");

            var clients = db.Clients.Include(c => c.User).ToList();
            return View(clients);
        }

        // GET: Clients/Details/5
        public ActionResult Details(string id)
        {
            if (!HasPermission("ViewClient"))
                return new HttpUnauthorizedResult("No tiene permiso para ver detalles de clientes.");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var client = db.Clients.Find(id);
            if (client == null)
                return HttpNotFound();

            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create(int? userId)
        {
            if (!HasPermission("CreateClient"))
                return new HttpUnauthorizedResult("No tiene permiso para crear clientes.");

            const int CLIENT_ROLE_ID = 3;

            if (userId.HasValue)
            {
                var usuario = db.Users.Find(userId);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "El usuario seleccionado no existe.";
                    return RedirectToAction("Index");
                }

                if (usuario.RoleID != CLIENT_ROLE_ID)
                {
                    TempData["ErrorMessage"] = "El usuario no tiene rol de Cliente.";
                    return RedirectToAction("Index");
                }

                var clienteExistente = db.Clients.FirstOrDefault(c => c.UserID == userId);
                if (clienteExistente != null)
                {
                    TempData["ErrorMessage"] = "El usuario ya está asociado a un cliente.";
                    return RedirectToAction("Index");
                }

                ViewBag.UserName = usuario.UserName;
                ViewBag.UserID = usuario.ID;
            }
            else
            {
                var usuariosDisponibles = db.Users
                    .Where(u => u.RoleID == CLIENT_ROLE_ID && !db.Clients.Any(c => c.UserID == u.ID))
                    .ToList();

                if (!usuariosDisponibles.Any())
                {
                    TempData["ErrorMessage"] = "No hay usuarios Client disponibles para asociar.";
                    return RedirectToAction("Index");
                }

                ViewBag.Users = new SelectList(usuariosDisponibles, "ID", "UserName");
            }

            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDNumber,FirstName,LastName,Email,BirthDate,UserID")] Client client)
        {
            if (!HasPermission("CreateClient"))
                return new HttpUnauthorizedResult("No tiene permiso para crear clientes.");

            try
            {
                if (!EsCedulaValida(client.IDNumber))
                    ModelState.AddModelError("IDNumber", "La cédula uruguaya no es válida.");

                if (!string.IsNullOrEmpty(client.IDNumber))
                {
                    if (db.Clients.Find(client.IDNumber) != null)
                        ModelState.AddModelError("IDNumber", "Ya existe un cliente con esta cédula.");
                }

                if (!string.IsNullOrEmpty(client.Email))
                {
                    if (db.Clients.Any(c => c.Email == client.Email))
                        ModelState.AddModelError("Email", "Ya existe un cliente con este correo.");
                }

                if (client.UserID.HasValue)
                {
                    var usuario = db.Users.Find(client.UserID);
                    if (usuario == null || usuario.RoleID != 3)
                        ModelState.AddModelError("UserID", "El usuario no es válido o no tiene rol Client.");

                    if (db.Clients.Any(c => c.UserID == client.UserID))
                        ModelState.AddModelError("UserID", "El usuario ya está asociado a otro cliente.");
                }
                else
                {
                    ModelState.AddModelError("UserID", "Debe seleccionar un usuario.");
                }

                if (ModelState.IsValid)
                {
                    db.Clients.Add(client);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cliente creado exitosamente.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en POST Create: " + ex.Message);
                ModelState.AddModelError("", "Error al guardar el cliente.");
            }

            // Recargar selección para vista
            const int CLIENT_ROLE_ID = 3;
            var usuariosDisponibles = db.Users
                .Where(u => u.RoleID == CLIENT_ROLE_ID && !db.Clients.Any(c => c.UserID == u.ID))
                .ToList();

            ViewBag.Users = new SelectList(usuariosDisponibles, "ID", "UserName", client.UserID);

            return View(client);
        }

        // GET: Clients/Edit/5
        // GET: Clients/Edit/5
        public ActionResult Edit(string id)
        {
            if (!HasPermission("EditClient"))
                return new HttpUnauthorizedResult("No tiene permiso para editar clientes.");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var client = db.Clients.Find(id);
            if (client == null)
                return HttpNotFound();

            // Generar lista de usuarios válidos:
            const int CLIENT_ROLE_ID = 3;

            var usuariosDisponibles = db.Users
                .Where(u => u.RoleID == CLIENT_ROLE_ID &&
                       (!db.Clients.Any(c => c.UserID == u.ID) || u.ID == client.UserID))
                .ToList();

            ViewBag.UserID = new SelectList(usuariosDisponibles, "ID", "UserName", client.UserID);

            return View(client);
        }


        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDNumber,FirstName,LastName,Email,BirthDate,UserID")] Client client)
        {
            if (!HasPermission("EditClient"))
                return new HttpUnauthorizedResult("No tiene permiso para editar clientes.");

            if (!EsCedulaValida(client.IDNumber))
                ModelState.AddModelError("IDNumber", "La cédula uruguaya no es válida.");

            if (db.Clients.Any(c => c.Email == client.Email && c.IDNumber != client.IDNumber))
                ModelState.AddModelError("Email", "Ya existe otro cliente con este correo.");

            if (client.UserID.HasValue)
            {
                const int CLIENT_ROLE_ID = 3;
                var usuario = db.Users.Find(client.UserID);
                if (usuario == null || usuario.RoleID != CLIENT_ROLE_ID)
                    ModelState.AddModelError("UserID", "El usuario no es válido o no tiene rol Client.");

                if (db.Clients.Any(c => c.UserID == client.UserID && c.IDNumber != client.IDNumber))
                    ModelState.AddModelError("UserID", "El usuario ya está asociado a otro cliente.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cliente editado correctamente.";
                return RedirectToAction("Index");
            }

            // Si hay error de validación → reconstruir lista
            const int CLIENT_ROLE_ID_EDIT = 3;
            var usuariosDisponibles = db.Users
                .Where(u => u.RoleID == CLIENT_ROLE_ID_EDIT &&
                       (!db.Clients.Any(c => c.UserID == u.ID) || u.ID == client.UserID))
                .ToList();

            ViewBag.UserID = new SelectList(usuariosDisponibles, "ID", "UserName", client.UserID);

            return View(client);
        }


        // GET: Clients/Delete/5
        public ActionResult Delete(string id)
        {
            if (!HasPermission("DeleteClient"))
                return new HttpUnauthorizedResult("No tiene permiso para eliminar clientes.");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var client = db.Clients.Find(id);
            if (client == null)
                return HttpNotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!HasPermission("DeleteClient"))
                return new HttpUnauthorizedResult("No tiene permiso para eliminar clientes.");

            var client = db.Clients.Find(id);
            if (client == null)
                return HttpNotFound();

            db.Clients.Remove(client);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Cliente eliminado correctamente.";
            return RedirectToAction("Index");
        }

        private bool EsCedulaValida(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula)) return false;

            cedula = cedula.Replace(".", "").Replace("-", "").Trim();
            while (cedula.Length < 8) cedula = "0" + cedula;

            int[] coef = { 2, 9, 8, 7, 6, 3, 4 };
            int suma = 0;

            for (int i = 0; i < coef.Length; i++)
                suma += coef[i] * int.Parse(cedula[i].ToString());

            int resto = suma % 10;
            int digito = resto == 0 ? 0 : 10 - resto;

            return digito == int.Parse(cedula[7].ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
