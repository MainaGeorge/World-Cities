using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorldCities.Data.Entity_Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("iso2")]
        public string IS02 { get; set; }

        [JsonPropertyName("iso3")]
        public string IS03 { get; set; }
        public virtual List<City> Cities { get; set; }

    }
}
