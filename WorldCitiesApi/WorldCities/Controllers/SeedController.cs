using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using WorldCities.Data;
using WorldCities.Data.Entity_Models;

namespace WorldCities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");

            var path = Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");

            await using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);

            var worksheet = excelPackage.Workbook.Worksheets[0];

            var endRow = worksheet.Dimension.End.Row;

            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            var countriesByName = await _context.Countries
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase);

            for (var nRow = 2; nRow <= endRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
                var countryName = row[nRow, 5].GetValue<string>();
                var is02 = row[nRow, 6].GetValue<string>();
                var is03 = row[nRow, 7].GetValue<string>();

                if(countriesByName.ContainsKey(countryName)) continue;

                var country = new Country()
                {
                    Name = countryName,
                    IS02 = is02,
                    IS03 = is03
                };


                await _context.Countries.AddAsync(country);

                countriesByName.Add(countryName, country);

                numberOfCountriesAdded++;
            }

            if (numberOfCountriesAdded > 0) await _context.SaveChangesAsync();

            var cities = _context.Cities
                .AsNoTracking()
                .ToDictionary(x =>
                    (Name: x.Name, Latitude: x.Latitude, Longitude: x.Longitude, CountryId: x.CountryId));

            for (var nRow = 2; nRow <= endRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                var countryId = countriesByName[countryName].Id;

                if (cities.ContainsKey((Name: name, Latitude: lat, Longitude: lon, CountryId: countryId))) continue;

                var city = new City()
                {
                    Name = name,
                    Name_ASCII = nameAscii,
                    Latitude = lat,
                    Longitude = lon,
                    CountryId = countryId
                };

                await _context.Cities.AddAsync(city);

                numberOfCitiesAdded++;
            }

            if (numberOfCitiesAdded > 0)
                await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded

            });
        }
    }
}
