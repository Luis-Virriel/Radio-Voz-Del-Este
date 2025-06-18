using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class CurrencyData
    {

        [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Success { get; set; }

        [JsonProperty("terms", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Terms { get; set; }

        [JsonProperty("privacy", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Privacy { get; set; }

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public long? Timestamp { get; set; }

        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("quotes", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, double> Quotes { get; set; }

        public static CurrencyData FromJson(string json) =>
            JsonConvert.DeserializeObject<CurrencyData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CurrencyData self) =>
            JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }
}