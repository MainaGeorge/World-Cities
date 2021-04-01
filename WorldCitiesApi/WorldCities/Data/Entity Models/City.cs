using System.Diagnostics.CodeAnalysis;

namespace WorldCities.Data.Entity_Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Name_ASCII { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
    }
}
