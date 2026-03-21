using JobPortal.Infrastructure.Data;
using JobPortal.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.Controllers
{
    public class LocationController : Controller
    {

      
        private readonly AppDbContext _db;
      

        public LocationController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetCities(int stateId)
        {
            var cities = await _db.Cities
                .Where(c => c.StateId == stateId)
                .Select(c => new { id = c.Id, name = c.Name })
                .ToListAsync();
            return Json(cities);
        }
    }
}
