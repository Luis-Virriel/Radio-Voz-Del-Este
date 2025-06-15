using ObligatorioProgramacion3_Francisco_Luis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ObligatorioProgramacion3_Francisco_Luis.Controllers
{
    public class HomeController : Controller
    {
        // Página principal accesible para todos (anónimo y autenticado)
        private RadioEntities db = new RadioEntities();
        private readonly string _weatherApiKey = "40ed30cc4da04b0fed28c1dd01a0e483";

        [AllowAnonymous]
        public async Task<ActionResult> Index()
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
                var auspiciantesPath = HostingEnvironment.MapPath("~/Assets/auspiciantes");
                var auspiciantesLogos = new List<string>();

                if (Directory.Exists(auspiciantesPath))
                {
                    var files = Directory.GetFiles(auspiciantesPath);
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        auspiciantesLogos.Add(Url.Content("~/Assets/auspiciantes/" + fileName));
                    }
                }
                model.AuspiciantesLogos = auspiciantesLogos;
                // Cargar datos del clima
                try
                {
                    model.CurrentWeather = await GetCurrentWeatherAsync();
                    model.WeatherForecast = await GetWeatherForecastAsync();
                }
                catch (Exception ex)
                {
                    // Log del error - el clima no se mostrará si falla
                    System.Diagnostics.Debug.WriteLine($"Error al obtener clima: {ex.Message}");
                    // Puedes también agregar el error al ViewBag si quieres mostrarlo
                    ViewBag.WeatherError = "No se pudo cargar la información del clima";
                }

                ViewBag.Message = "Bienvenido visitante, por favor inicie sesión para más funciones";
                return View("Index", model);
            }
        }

        private async Task<WeatherViewModel> GetCurrentWeatherAsync()
        {
            using (var client = new HttpClient())
            {
                // Coordenadas de Maldonado, Uruguay
                var lat = -34.9;
                var lon = -54.95;

                var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_weatherApiKey}&units=metric&lang=es";

                var response = await client.GetStringAsync(url);
                return WeatherViewModel.FromJson(response);
            }
        }

        private async Task<List<WeatherForecastItem>> GetWeatherForecastAsync()
        {
            using (var client = new HttpClient())
            {
                var lat = -34.9;
                var lon = -54.95;

                var url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_weatherApiKey}&units=metric&lang=es";

                var response = await client.GetStringAsync(url);
                var forecastData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);

                var forecast = new List<WeatherForecastItem>();

                // Procesar solo un pronóstico por día (mediodía)
                var dailyForecasts = ((Newtonsoft.Json.Linq.JArray)forecastData.list)
                    .Where(item => item["dt_txt"].ToString().Contains("12:00:00"))
                    .Take(5);

                foreach (var item in dailyForecasts)
                {
                    forecast.Add(new WeatherForecastItem
                    {
                        Date = DateTime.Parse(item["dt_txt"].ToString()),
                        TempMax = (double?)item["main"]["temp_max"],
                        TempMin = (double?)item["main"]["temp_min"],
                        Description = item["weather"][0]["description"].ToString(),
                        Icon = item["weather"][0]["icon"].ToString()
                    });
                }

                return forecast;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetWeather()
        {
            try
            {
                var weather = await GetCurrentWeatherAsync();
                return Json(new
                {
                    success = true,
                    temperature = weather.Main?.Temp?.ToString("0"),
                    description = weather.Weather?.FirstOrDefault()?.Description,
                    icon = weather.Weather?.FirstOrDefault()?.Icon
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
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