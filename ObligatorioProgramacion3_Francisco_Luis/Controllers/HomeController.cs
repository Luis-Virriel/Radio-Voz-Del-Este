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
        private RadioEntities db = new RadioEntities();
        private readonly string _weatherApiKey = "40ed30cc4da04b0fed28c1dd01a0e483";
        private readonly string currencyLayerApiKey = "ba030d742f03dfc719d832c8010a3f84";




        public async Task<CurrencyData> GetCurrencyDataAsync()
        {
            using (var client = new HttpClient())
            {
               
                var url = $"http://api.currencylayer.com/live?access_key={currencyLayerApiKey}&currencies=EUR,UYU,BRL&source=USD&format=1";
                var response = await client.GetStringAsync(url);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyData>(response);
                return data;
            }
        }


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

                // Logos de auspiciantes
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

                // Datos del clima
                try
                {
                    model.CurrentWeather = await GetCurrentWeatherAsync();
                    model.WeatherForecast = await GetWeatherForecastAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al obtener clima: {ex.Message}");
                    ViewBag.WeatherError = "No se pudo cargar la información del clima";
                }

                // Cotización del dólar (desde base)
                var lastUsdRate = db.ExchangeRates
                    .Where(r => r.CurrencyType == "USD")
                    .OrderByDescending(r => r.ExchangeDate)
                    .Select(r => r.ExchangeValue)
                    .FirstOrDefault();

                model.UsdExchangeRate = lastUsdRate;

                // Cotización del dólar (desde API CurrencyLayer)
                try
                {
                    model.CurrencyData = await GetCurrencyDataAsync();

                    if (model.CurrencyData != null && model.CurrencyData.Quotes != null)
                    {
                        model.SelectedCurrencyQuotes = new Dictionary<string, double>();

                        // Claves esperadas (USD base, no es necesario USDUSD)
                        var keys = new string[] { "USDEUR", "USDUYU", "USDBRL" };


                        foreach (var key in keys)
                        {
                            if (model.CurrencyData.Quotes.ContainsKey(key))
                            {
                                model.SelectedCurrencyQuotes[key] = model.CurrencyData.Quotes[key];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al obtener moneda: {ex.Message}");
                    model.CurrencyData = null;
                    model.SelectedCurrencyQuotes = null;
                }

                ViewBag.Message = "Bienvenido visitante, por favor inicie sesión para más funciones";
                return View("Index", model);
            }
        }


        public async Task<WeatherViewModel> GetCurrentWeatherAsync()
        {
            using (var client = new HttpClient())
            {
                var lat = -34.9;
                var lon = -54.95;

                var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_weatherApiKey}&units=metric&lang=es";

                var response = await client.GetStringAsync(url);
                return WeatherViewModel.FromJson(response);
            }
        }

        public async Task<List<WeatherForecastItem>> GetWeatherForecastAsync()
        {
            using (var client = new HttpClient())
            {
                var lat = -34.9;
                var lon = -54.95;

                var url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_weatherApiKey}&units=metric&lang=es";

                var response = await client.GetStringAsync(url);
                var forecastData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);

                var forecast = new List<WeatherForecastItem>();

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
        public async Task<ActionResult> Weather()
        {
            try
            {
                var weather = await GetCurrentWeatherAsync();
                var forecast = await GetWeatherForecastAsync();

                var model = new WeatherDetailsViewModel
                {
                    CurrentWeather = weather,
                    Forecast = forecast
                };

                ViewBag.Message = "Información del clima actual y pronóstico.";
                return View("Weather", model); // ← usa Weather.cshtml
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo obtener la información del clima.";
                return View("Weather", null);
            }
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
        public async Task<ActionResult> Currency()
        {
            var model = new HomeIndexViewModel();

            try
            {
                model.CurrencyData = await GetCurrencyDataAsync();

                if (model.CurrencyData != null && model.CurrencyData.Quotes != null)
                {
                    var quotes = model.CurrencyData.Quotes;

                    if (quotes.ContainsKey("USDUYU"))
                    {
                        double usdUYU = quotes["USDUYU"];
                        model.SelectedCurrencyQuotes = new Dictionary<string, double>();

                        // USD → UYU (se muestra como UYU/USD)
                        model.SelectedCurrencyQuotes["UYU/USD"] = usdUYU;

                        // EUR → UYU
                        if (quotes.ContainsKey("USDEUR"))
                        {
                            double eurUYU = usdUYU / quotes["USDEUR"];
                            model.SelectedCurrencyQuotes["UYU/EUR"] = eurUYU;
                        }

                        // BRL → UYU
                        if (quotes.ContainsKey("USDBRL"))
                        {
                            double brlUYU = usdUYU / quotes["USDBRL"];
                            model.SelectedCurrencyQuotes["UYU/BRL"] = brlUYU;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo obtener información de cotizaciones.";
                model.CurrencyData = null;
                model.SelectedCurrencyQuotes = null;
            }

            return View(model);
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