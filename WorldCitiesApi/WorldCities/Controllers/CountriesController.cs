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
    public class CountriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<ApiResult<CountryDto>>> GetCountries(int pageIndex = 0, int pageSize = 10, string sortColumn = null,
            string sortOrder = null, string filterColumn = null, string filterQuery = null)
        {
            return await ApiResult<CountryDto>.CreateAsync(_context.Countries.Select(c => new CountryDto
            {
                Id = c.Id,
                IS02 = c.IS02,
                IS03 = c.IS03,
                TotCities = c.Cities.Count,
                Name = c.Name
            }),
                pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("IsDuplicateField")]
        public async Task<bool> IsDuplicateField(int countryId, string fieldName, string fieldValue)
        {
            return fieldName switch
            {
                "name" => await _context.Countries.AnyAsync(c => c.Name == fieldValue && c.Id != countryId),
                "iso2" => await _context.Countries.AnyAsync(c => c.IS02 == fieldValue && c.Id != countryId),
                "iso3" => await _context.Countries.AnyAsync(c => c.IS03 == fieldValue && c.Id != countryId),
                _ => false
            };
        }
        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
