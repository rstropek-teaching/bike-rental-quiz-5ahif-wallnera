using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bikerental.Context;
using Bikerental.Model;

namespace Bikerental.Controllers
{
    [Produces("application/json")]
    [Route("api/customers")]
    public class CustomersController : Controller
    {
        private readonly DataContext _context;

        public CustomersController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get all customers
        /// </summary>
        /// <returns></returns>
        // GET: api/customers
        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(_context.Customers.ToArray());
        }

        /// <summary>
        /// get customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.SingleOrDefaultAsync(cst => cst.ID == id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        // PUT: api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer([FromRoute] int id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.ID)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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
        /// Add customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        // POST: api/customers
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok("Customer added");
        }

        /// <summary>
        /// delete customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.SingleOrDefaultAsync(cst => cst.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            if(_context.Rentals.Any(rnt => rnt.Customer.ID == id))
            {
                return BadRequest("Can't delete Customer with rentals!");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        /// <summary>
        /// Get all Rentals for a Customer (Id needed)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/customers/5
        [HttpGet]
        [Route("GetCustomerRentals/{id}")]
        public IActionResult GetCustomerRentals(int id)
        {
            if(!_context.Customers.Any(cst => cst.ID == id))
            {
                return BadRequest("No Customer with this Id");
            }
            return Ok(_context.Rentals.Where(rnt => rnt.Customer.ID == id));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.ID == id);
        }
    }
}