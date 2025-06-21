using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class WeatherDetailsViewModel
    {
        public WeatherViewModel CurrentWeather { get; set; }
        public List<WeatherForecastItem> Forecast { get; set; }
    }
}
