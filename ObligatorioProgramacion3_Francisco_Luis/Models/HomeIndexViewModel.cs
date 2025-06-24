using System.Collections.Generic;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class HomeIndexViewModel
    {
        // Propiedades existentes para programas de radio, clima, etc.
        public RadioProgram CurrentProgram { get; set; }
        public RadioProgram NextProgram { get; set; }
        public List<RadioProgram> ProgramsList { get; set; }

        public WeatherViewModel CurrentWeather { get; set; }
        public List<WeatherForecastItem> WeatherForecast { get; set; }

        public bool HasWeatherData => CurrentWeather != null;
        public bool HasForecastData => WeatherForecast != null && WeatherForecast.Count > 0;

        public string CurrentTemperature => CurrentWeather?.Main?.Temp?.ToString("0") + "°C";
        public string CurrentDescription => CurrentWeather?.Weather?[0]?.Description;
        public string CurrentWeatherIcon => CurrentWeather?.Weather?[0]?.Icon;
        public string CurrentWeatherIconUrl => HasWeatherData && !string.IsNullOrEmpty(CurrentWeatherIcon)
            ? $"https://openweathermap.org/img/wn/{CurrentWeatherIcon}@2x.png"
            : "";

        public decimal? UsdExchangeRate { get; set; }
        public List<string> AuspiciantesLogos { get; set; }

        public CurrencyData CurrencyData { get; set; }

        // Esta propiedad es la que faltaba:
        public bool HasCurrencyData => CurrencyData != null && CurrencyData.Quotes != null;

        // NUEVA propiedad para cotizaciones seleccionadas
        public Dictionary<string, double> SelectedCurrencyQuotes { get; set; }
    }
}
