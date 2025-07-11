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
        [Authorize]
        public ActionResult Sponsors()
        {
            try
            {
                var sponsorsFromDb = db.Sponsors.OrderBy(s => s.SponsorsName).ToList();

                var viewModel = new SponsorsViewModel
                {
                    Sponsors = sponsorsFromDb.Select(s => new SponsorItem
                    {
                        ID = s.ID,
                        SponsorsName = s.SponsorsName,
                        SponsorsDescription = s.SponsorsDescription,
                        CantPlan = (int)s.CantPlan
                    }).ToList(),
                    TotalSponsors = sponsorsFromDb.Count,
                    TotalPlans = (int)sponsorsFromDb.Sum(s => s.CantPlan),
                    AveragePlans = (double)(sponsorsFromDb.Any() ? sponsorsFromDb.Average(s => s.CantPlan) : 0)
                };

                ViewBag.Title = "Nuestros Patrocinadores - Voz del Este";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando sponsors: {ex.Message}");
                ViewBag.Error = "Error al cargar los patrocinadores. Por favor, intenta nuevamente.";

                var emptyViewModel = new SponsorsViewModel();
                return View(emptyViewModel);
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

                var lastUsdRate = db.ExchangeRates
                    .Where(r => r.CurrencyType == "USD")
                    .OrderByDescending(r => r.ExchangeDate)
                    .Select(r => r.ExchangeValue)
                    .FirstOrDefault();

                model.UsdExchangeRate = lastUsdRate;

                try
                {
                    model.CurrencyData = await GetCurrencyDataAsync();

                    if (model.CurrencyData != null && model.CurrencyData.Quotes != null)
                    {
                        model.SelectedCurrencyQuotes = new Dictionary<string, double>();

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
        [Authorize]
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
        [Authorize]

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
                return View("Weather", model);
            }
            catch (Exception ex)
            {
                ex.Source.ToString();
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
        [Authorize]

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

                        model.SelectedCurrencyQuotes["UYU/USD"] = usdUYU;

                        if (quotes.ContainsKey("USDEUR"))
                        {
                            double eurUYU = usdUYU / quotes["USDEUR"];
                            model.SelectedCurrencyQuotes["UYU/EUR"] = eurUYU;
                        }

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
                ex.Message.ToString();
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
        [Authorize]

        public ActionResult News(string category = null, string title = null)
        {
            try
            {
                var newsFromDb = db.News.OrderByDescending(n => n.PublishDate).ToList();

                var newsItems = newsFromDb.Select(n => {
                    var newsItem = new Models.NewsItem
                    {
                        Id = (int)(n.Id ?? 0),
                        Title = n.Title ?? "Sin título",
                        Content = n.Content ?? "Sin contenido",
                        PublishDate = n.PublishDate ?? DateTime.MinValue,
                        ImageURL = n.ImageURL
                    };

                    newsItem.SetCategoryFromTitle();
                    newsItem.SetRandomAuthor();
                    newsItem.SetDefaultImageIfEmpty();

                    return newsItem;
                }).ToList();

                var additionalNews = GetAdditionalDemoNews();
                newsItems.AddRange(additionalNews);

                newsItems = newsItems.OrderByDescending(n => n.PublishDate).ToList();

                if (!string.IsNullOrEmpty(category))
                {
                    newsItems = newsItems.Where(n => n.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (string.IsNullOrEmpty(category) && newsItems.Any())
                {
                    newsItems.First().IsFeatured = true;
                }

                var allNews = newsFromDb.Select(n => {
                    var newsItem = new Models.NewsItem
                    {
                        Id = (int)(n.Id ?? 0),
                        Title = n.Title ?? "Sin título",
                        Content = n.Content ?? "Sin contenido",
                        PublishDate = n.PublishDate ?? DateTime.MinValue,
                        ImageURL = n.ImageURL
                    };
                    newsItem.SetCategoryFromTitle();
                    return newsItem;
                }).ToList();

                allNews.AddRange(GetAdditionalDemoNews());
                var trendingNews = allNews.OrderByDescending(n => n.PublishDate).Take(5).ToList();

                var viewModel = new NewsViewModel
                {
                    NewsItems = newsItems,
                    TrendingNews = trendingNews,
                    SelectedCategory = category,
                    TotalCount = newsItems.Count,
                    CurrentPage = 1
                };

                ViewBag.SelectedCategory = category;
                ViewBag.Title = !string.IsNullOrEmpty(category) ? $"Noticias - {category}" : "Todas las Noticias";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en News: {ex.Message}");

                var emptyViewModel = new NewsViewModel();
                ViewBag.Error = "Error al cargar las noticias. Por favor, intenta nuevamente.";

                return View(emptyViewModel);
            }
        }
        [Authorize]

        public ActionResult RadioPrograms()
        {
            try
            {
                var model = new RadioProgramsViewModel();

                var allPrograms = db.RadioPrograms.OrderBy(p => p.ProgramName).ToList();

                DateTime ahora = DateTime.Now;
                var currentProgram = db.RadioPrograms
                    .Where(p => p.Schedule <= ahora && DbFunctions.AddHours(p.Schedule, 1) > ahora)
                    .OrderBy(p => p.Schedule)
                    .FirstOrDefault();

                if (currentProgram == null && allPrograms.Any())
                {
                    var random = new Random();
                    currentProgram = allPrograms[random.Next(allPrograms.Count)];
                    currentProgram.Schedule = DateTime.Now;
                }

                model.AllPrograms = allPrograms;
                model.CurrentProgram = currentProgram;
                model.TotalPrograms = allPrograms.Count;

                ViewBag.Title = "Programas de Radio - Voz del Este";
                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando programas: {ex.Message}");
                ViewBag.Error = "Error al cargar los programas. Por favor, intenta nuevamente.";

                var emptyModel = new RadioProgramsViewModel
                {
                    AllPrograms = new List<RadioProgram>(),
                    CurrentProgram = null,
                    TotalPrograms = 0
                };

                return View(emptyModel);
            }
        }

        private List<NewsItem> GetAdditionalDemoNews()
        {
            var demoNews = new List<NewsItem>
            {
                new NewsItem
                {
                    Id = -1,
                    Title = "Festival de Jazz de Maldonado anuncia su programación 2025",
                    Content = "Artistas internacionales como Brad Mehldau y Esperanza Spalding encabezarán el evento que se realizará del 15 al 20 de febrero en diferentes espacios de la ciudad. El festival incluirá masterclasses gratuitas y conciertos al aire libre en el puerto de Punta del Este.",
                    PublishDate = DateTime.Now.AddHours(-12),
                    ImageURL = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=220&fit=crop",
                    Category = "Cultura",
                    Author = "Diego Ruiz",
                    AuthorRole = "Crítico Musical"
                },
                new NewsItem
                {
                    Id = -2,
                    Title = "Nuevo shopping center en Maldonado genera 800 empleos directos",
                    Content = "La construcción del complejo comercial más grande de la región este avanza según cronograma. Se esperan 150 locales comerciales, cines y área de entretenimiento familiar. La inversión total asciende a $45 millones de dólares y se espera la inauguración para diciembre de 2025.",
                    PublishDate = DateTime.Now.AddHours(-18),
                    ImageURL = "https://images.unsplash.com/photo-1582719471137-c3967ffb1c42?w=400&h=220&fit=crop",
                    Category = "Local",
                    Author = "Patricia González",
                    AuthorRole = "Editora Local"
                },
                new NewsItem
                {
                    Id = -3,
                    Title = "Suárez confirma su permanencia en Nacional hasta 2026",
                    Content = "El delantero uruguayo renovó su contrato con el club tricolor hasta diciembre de 2026. La decisión llega tras las especulaciones sobre ofertas del exterior. El jugador expresó su compromiso con el equipo y los objetivos deportivos para la próxima temporada.",
                    PublishDate = DateTime.Now.AddHours(-20),
                    ImageURL = "https://images.unsplash.com/photo-1551698618-1dfe5d97d256?w=400&h=220&fit=crop",
                    Category = "Deportes",
                    Author = "Juan López",
                    AuthorRole = "Corresponsal Deportivo"
                },
                new NewsItem
                {
                    Id = -4,
                    Title = "Récord histórico de turistas brasileños en Punta del Este",
                    Content = "La temporada 2024-2025 registra un incremento del 35% en visitantes provenientes de Brasil. Los vuelos charter directos desde São Paulo y Río de Janeiro han facilitado el acceso al destino. Hoteleros reportan ocupación plena hasta marzo con una derrama económica estimada en $180 millones.",
                    PublishDate = DateTime.Now.AddDays(-1),
                    ImageURL = "https://images.unsplash.com/photo-1540979388789-6cee28a1cdc9?w=400&h=220&fit=crop",
                    Category = "Turismo",
                    Author = "Carlos Méndez",
                    AuthorRole = "Especialista en Turismo"
                },
                new NewsItem
                {
                    Id = -5,
                    Title = "Nueva ruta costera conectará Maldonado con Rocha",
                    Content = "El proyecto vial de $25 millones incluye 45 kilómetros de autopista moderna con miradores panorámicos. La obra beneficiará el turismo regional y reducirá los tiempos de viaje en un 40%. El inicio de construcción está previsto para abril con finalización en 18 meses.",
                    PublishDate = DateTime.Now.AddDays(-2),
                    ImageURL = "https://images.unsplash.com/photo-1516205651411-aef33a44f7c2d?w=400&h=220&fit=crop",
                    Category = "Local",
                    Author = "Roberto Santos",
                    AuthorRole = "Corresponsal"
                },
                new NewsItem
                {
                    Id = -6,
                    Title = "Festival de cine latinoamericano llega a Punta del Este",
                    Content = "Del 5 al 12 de marzo se realizará la primera edición del Festival Internacional de Cine Latinoamericano. Se proyectarán 60 películas de 15 países en el Enjoy Conrad y el nuevo centro cultural. Habrá competencias, talleres y charlas con directores reconocidos.",
                    PublishDate = DateTime.Now.AddDays(-3),
                    ImageURL = "https://images.unsplash.com/photo-1494526585095-c41746248156?w=400&h=220&fit=crop",
                    Category = "Cultura",
                    Author = "María Pérez",
                    AuthorRole = "Editora Cultural"
                }
            };

            return demoNews;
        }

        // NUEVA ACCIÓN para mostrar siempre la vista general Voz del Este
        public async Task<ActionResult> Welcome()
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

            try
            {
                model.CurrentWeather = await GetCurrentWeatherAsync();
                model.WeatherForecast = await GetWeatherForecastAsync();
            }
            catch
            {
                ViewBag.WeatherError = "No se pudo cargar la información del clima";
            }

            var lastUsdRate = db.ExchangeRates
                .Where(r => r.CurrencyType == "USD")
                .OrderByDescending(r => r.ExchangeDate)
                .Select(r => r.ExchangeValue)
                .FirstOrDefault();

            model.UsdExchangeRate = lastUsdRate;

            try
            {
                model.CurrencyData = await GetCurrencyDataAsync();

                if (model.CurrencyData != null && model.CurrencyData.Quotes != null)
                {
                    model.SelectedCurrencyQuotes = new Dictionary<string, double>();
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
            catch
            {
                model.CurrencyData = null;
                model.SelectedCurrencyQuotes = null;
            }

            ViewBag.Message = "Bienvenido visitante, por favor inicie sesión para más funciones";
            return View("Index", model);
        }
    }
}
