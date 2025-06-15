using System.Collections.Generic;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class HomeIndexViewModel
    {
        // Propiedades existentes para programas de radio
        public RadioProgram CurrentProgram { get; set; }
        public RadioProgram NextProgram { get; set; }
        public List<RadioProgram> ProgramsList { get; set; }

        // Nuevas propiedades para el clima
        public WeatherViewModel CurrentWeather { get; set; }
        public List<WeatherForecastItem> WeatherForecast { get; set; }

        // Propiedades de conveniencia para la vista
        public bool HasWeatherData => CurrentWeather != null;
        public bool HasForecastData => WeatherForecast != null && WeatherForecast.Count > 0;

        public string CurrentTemperature => CurrentWeather?.Main?.Temp?.ToString("0") + "°C";
        public string CurrentDescription => CurrentWeather?.Weather?[0]?.Description;
        public string CurrentWeatherIcon => CurrentWeather?.Weather?[0]?.Icon;
        public string CurrentWeatherIconUrl => HasWeatherData && !string.IsNullOrEmpty(CurrentWeatherIcon)
            ? $"https://openweathermap.org/img/wn/{CurrentWeatherIcon}@2x.png"
            : "";
        public List<string> AuspiciantesLogos { get; set; }
    }
}