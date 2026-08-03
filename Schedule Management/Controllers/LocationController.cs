using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Models;

namespace Schedule_Management.Controllers
{
    public class LocationController : Controller
    {
        private readonly ScheduleManagementDbContext _context;

        public LocationController(
            ScheduleManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _context.Countries
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CountryName)
                .Select(c => new
                {
                    c.CountryId,
                    c.CountryName
                })
                .ToListAsync();

            return Json(countries);
        }

        [HttpGet]
        public async Task<IActionResult> GetStates(
            int countryId)
        {
            var states = await _context.States
                .AsNoTracking()
                .Where(s =>
                    s.CountryId == countryId &&
                    s.IsActive)
                .OrderBy(s => s.StateName)
                .Select(s => new
                {
                    s.StateId,
                    s.StateName
                })
                .ToListAsync();

            return Json(states);
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(
            int stateId)
        {
            var districts = await _context.Districts
                .AsNoTracking()
                .Where(d =>
                    d.StateId == stateId &&
                    d.IsActive)
                .OrderBy(d => d.DistrictName)
                .Select(d => new
                {
                    d.DistrictId,
                    d.DistrictName
                })
                .ToListAsync();

            return Json(districts);
        }
    }
}