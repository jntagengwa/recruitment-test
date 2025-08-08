using InterviewTest.Server.Data;
using InterviewTest.Server.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterviewTest.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            _logger.LogInformation("Retrieved {Count} employees", employees.Count);
            return employees;
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Employee {Id} retrieved successfully", id);
            return employee;
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.Name))
                return BadRequest("Name is required.");

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new employee {Name} with ID {Id}", employee.Name, employee.Id);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
                return BadRequest("ID mismatch.");

            var exists = await _context.Employees.AnyAsync(e => e.Id == id);
            if (!exists)
                return NotFound();

            _logger.LogInformation("Updating employee {Id}", id);

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating employee {Id}", id);
                return StatusCode(500, "A concurrency error occurred.");
            }

            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Attempted to delete non-existent employee {Id}", id);
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted employee {Id}", id);

            return NoContent();
        }
        [HttpPost("update-values-and-sum")]
        public async Task<ActionResult<object>> UpdateValuesAndReturnFilteredSum()
        {
            // Bulk update Values based on first letter of Name
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE Employees
                SET Value = 
                    CASE
                        WHEN Name LIKE 'E%' THEN Value + 1
                        WHEN Name LIKE 'G%' THEN Value + 10
                        ELSE Value + 100
                    END
            ");

            // Calculate the grouped sum for names starting with A/B/C
            var result = await _context.Employees
                .Where(e => EF.Functions.Like(e.Name, "A%") ||
                            EF.Functions.Like(e.Name, "B%") ||
                            EF.Functions.Like(e.Name, "C%"))
                .GroupBy(e => 1)
                .Select(g => new { Total = g.Sum(e => e.Value) })
                .FirstOrDefaultAsync();

            _logger.LogInformation("Performed bulk update and sum. Result total: {Total}", result?.Total ?? 0);

            if (result == null || result.Total < 11171)
                return NoContent();

            return Ok(new { SumOfABC = result.Total });
        }
    }
}
