using ObligatorioProgramacion3_Francisco_Luis.Models;

using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class HomeController : Controller
    {
        // Página principal accesible para todos (anónimo y autenticado)
        private RadioEntities db = new RadioEntities();

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var role = Session["Role"]?.ToString();

                if (role == "Admin")
                {
                    ViewBag.Message = "Bienvenido Admin";
                    return View("AdminIndex");
                }
                else if (role == "Editors")
                {
                    ViewBag.Message = "Bienvenido Editor";
                    return View("EditorIndex");
                }
                else if (role == "Clients")
                {
                    ViewBag.Message = "Bienvenido Cliente";
                    return View("ClientIndex");
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            else
            {
                DateTime ahora = DateTime.Now;

                var currentProgram = db.RadioPrograms
                    .Where(p => p.Schedule <= ahora && DbFunctions.AddHours(p.Schedule, 1) > ahora)
                    .OrderBy(p => p.Schedule)
                    .FirstOrDefault();

                var nextProgram = db.RadioPrograms
                    .Where(p => p.Schedule > ahora)
                    .OrderBy(p => p.Schedule)
                    .FirstOrDefault();

                var today = ahora.Date;

                var programsList = db.RadioPrograms
                    .Where(p => DbFunctions.TruncateTime(p.Schedule) == today)
                    .OrderBy(p => p.Schedule)
                    .ToList();

                var model = new HomeIndexViewModel
                {
                    CurrentProgram = currentProgram,
                    NextProgram = nextProgram,
                    ProgramsList = programsList
                };

                ViewBag.Message = "Bienvenido visitante, por favor inicie sesión para más funciones";

                return View("Index", model);
            }
        }

        [Authorize]
        public ActionResult ProtectedArea()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Descripción de la aplicación.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Página de contacto.";
            return View();
        }

        public ActionResult AdminIndex()
        {
            return View();
        }
        public ActionResult EditorIndex()
        {
            return View();
        }
        public ActionResult ClientIndex()
        {
            return View();
        }


    }
}

