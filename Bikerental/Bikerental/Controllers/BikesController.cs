using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bikerental.Context;
using Bikerental.Model;

namespace Bikerental.Controllers
{
    [Produces("application/json")]
    [Route("api/bikes")]
    public class BikesController : Controller
    {
        private readonly DataContext _context;

        public BikesController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get all bikes
        /// </summary>
        /// <returns></returns>
        // GET: api/bikes
        [HttpGet]
        public IActionResult GetBikes()
        {
            return Ok(_context.Bikes);
        }

        /// <summary>
        /// get available bikes
        /// </summary>
        /// <returns></returns>
        // GET: api/bikes
        [HttpGet]
        [Route("GetAvailableBikes")]
        public IActionResult GetAvailableBikes()
        {
            var res = _context.Bikes.Select(bike => bike).Where(bk => !_context.Rentals.Any(rnt => rnt.Bike.ID == bk.ID & rnt.RentalBegin > rnt.RentalEnd));
            return Ok(res);
        }

        /// <summary>
        /// Get bike by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/bikes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBike([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bike = await _context.Bikes.SingleOrDefaultAsync(m => m.ID == id);

            if (bike == null)
            {
                return NotFound();
            }

            return Ok(bike);
        }

        /// <summary>
        /// Update bike
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bike"></param>
        /// <returns></returns>
        // PUT: api/bikes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBike([FromRoute] int id, [FromBody] Bike bike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bike.ID)
            {
                return BadRequest();
            }

            _context.Entry(bike).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BikeExists(id))
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

        /// <summary>
        /// Add a bike
        /// </summary>
        /// <param name="bike"></param>
        /// <returns></returns>
        // POST: api/bikes
        [HttpPost]
        public async Task<IActionResult> AddBike([FromBody] Bike bike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Bikes.Add(bike);
            await _context.SaveChangesAsync();

            return Ok("Bike added");
        }

        /// <summary>
        /// delete a bike
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/bikes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBike([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bike = await _context.Bikes.SingleOrDefaultAsync(m => m.ID == id);
            if (bike == null)
            {
                return NotFound();
            }

            if(_context.Rentals.Any(bk=>bk.Bike.ID == id))
            {
                return BadRequest("Bike is in use!");
            }

            _context.Bikes.Remove(bike);
            await _context.SaveChangesAsync();

            return Ok("deleted");
        }

        private bool BikeExists(int id)
        {
            return _context.Bikes.Any(e => e.ID == id);
        }
    }
}