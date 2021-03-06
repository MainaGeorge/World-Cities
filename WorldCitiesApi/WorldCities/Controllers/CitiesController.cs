using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data;
using WorldCities.Data.Entity_Models;
using WorldCities.DTOs;

namespace WorldCities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("default")]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<ApiResult<CityDto>>> GetCities(int pageIndex = 0, int pageSize = 10,
            string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
        {
            return await ApiResult<CityDto>.CreateAsync(_context.Cities.Select(c => new CityDto
            {
                Name = c.Name,
                Id = c.Id,
                CountryId = c.Country.Id,
                CountryName = c.Country.Name,
                Latitude = c.Latitude,
                Longitude = c.Longitude
            }),
                pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [EnableCors("default")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [EnableCors("default")]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.Id }, city);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("IsDuplicateCity")]
        public async Task<bool> IsDuplicateCity(City city)
        {
            return await _context.Cities.AnyAsync(c =>

                c.CountryId == city.CountryId &&
                c.Name == city.Name &&
                c.Latitude == city.Latitude &&
                c.Longitude == city.Longitude &&
                c.Id != city.Id
            );
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
