using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bikerental.Context;
using Bikerental.Model;
using System;

namespace Bikerental.Controllers
{
    [Produces("application/json")]
    [Route("api/rentals")]
    public class RentalsController : Controller
    {
        private readonly DataContext _context;
        private BikeRentalLogic logic = new BikeRentalLogic();

        public RentalsController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all rentals
        /// </summary>
        /// <returns></returns>
        // GET: api/rentals
        [HttpGet]
        public IActionResult GetRentals()
        {
            return Ok(_context.Rentals);
        }

        /// <summary>
        /// Get rental by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/rentals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRental([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rental = await _context.Rentals.SingleOrDefaultAsync(rnt => rnt.ID == id);

            if (rental == null)
            {
                return NotFound();
            }

            return Ok(rental);
        }

        /// <summary>
        /// Update rental 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rental"></param>
        /// <returns></returns>
        // PUT: api/rentals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRental([FromRoute] int id, [FromBody] Rental rental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rental.ID)
            {
                return BadRequest();
            }

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
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
        /// Add a rental
        /// </summary>
        /// <param name="rental"></param>
        /// <returns></returns>
        // POST: api/rentals
        [HttpPost]
        public async Task<IActionResult> AddRental([FromBody] Rental rental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return Ok("Rental added");
        }

        /// <summary>
        /// Delete a rental
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/rentals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRental([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rental = await _context.Rentals.SingleOrDefaultAsync(rnt => rnt.ID == id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return Ok(rental);
        }

        /// <summary>
        /// Starts a rental
        /// </summary>
        /// <param name="idCustomer"></param>
        /// <param name="idBike"></param>
        /// <returns></returns>
        // GET: api/rentals/StartRental
        [HttpGet]
        [Route("StartRental")]
        public async Task<IActionResult> StartRentalAsync(int custId, int bikeId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_context.Customers.Any(cst => cst.ID == custId) || !_context.Bikes.Any(bk => bk.ID == bikeId))
            {
                return BadRequest("No valid IDs");
            }

            Rental newRental = new Rental()
            {
                Customer = _context.Customers.First(cst => cst.ID == custId),
                Bike = _context.Bikes.First(bk => bk.ID == bikeId),
                RentalBegin = DateTime.Now,
                RentalEnd = null,
                Paid = false,
                RentalCosts = 0
            };

            _context.Rentals.Add(newRental);
            await _context.SaveChangesAsync();

            return Ok("Rental has started " + newRental);
        }

        /// <summary>
        /// used to set a end for a rental and calculate the fee for the usage of the bike
        /// two parameters (if a customer has more rented bikes)
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="bikeId"></param>
        /// <returns></returns>
        // GET: api/rentals/EndRental
        [HttpGet]
        [Route("EndRental")]
        public async Task<IActionResult> EndRentalAsync(int custId, int bikeId)
        {
            double fee = 0.00d;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rent = _context.Rentals.First(rnt => rnt.Customer.ID == custId & rnt.Bike.ID == bikeId);
            rent.RentalEnd = DateTime.Now;

            fee =  logic.Calculate(rent);
            rent.RentalCosts = fee;

            await _context.SaveChangesAsync();

            return Ok(rent);
        }

        /// <summary>
        /// Used to mark a rental as paid
        /// two parameter, if customer hasn't paid the last payment but rented a bike again
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="bikeID"></param>
        /// <returns></returns>
        // GET: api/rentals/Pay
        [HttpGet]
        [Route("Pay")]
        public async Task<IActionResult> MarkPaidAsync(int custId, int bikeID)
        {
            var rent = _context.Rentals.First(rnt => rnt.Customer.ID == custId & rnt.Bike.ID == bikeID);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (rent.RentalCosts > 0)
            {
                rent.Paid = true;
            }
            else
            {
                return BadRequest("Customer has not paid");
            }
            await _context.SaveChangesAsync();

            return Ok("Customer (id=" + custId + "), Bike (id=" + bikeID + ") Paid");
        }

        /// <summary>
        /// returns all unpaid rentals
        /// </summary>
        /// <returns></returns>
        // GET: api/rentals/Unpaid
        [HttpGet]
        [Route("Unpaid")]
        public IActionResult GetUnpaid()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_context.Rentals.Where(rnt => rnt.Paid == false && rnt.RentalEnd > rnt.RentalBegin && rnt.RentalEnd != null && rnt.RentalCosts > 0).
                SelectMany(rntobj => new Object[] {
                    rntobj.Customer.ID,
                    rntobj.Customer.FirstName,
                    rntobj.Customer.LastName,
                    rntobj.ID,
                    rntobj.RentalBegin,
                    rntobj.RentalEnd,
                    rntobj.RentalCosts }
                ));
        }

        
        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.ID == id);
        }
    }
}