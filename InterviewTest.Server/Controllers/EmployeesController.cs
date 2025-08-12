using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using InterviewTest.Server.DTOs;
using InterviewTest.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterviewTest.Server.Controllers
{
    /// <summary>
    /// Exposes CRUD operations for employees and a special endpoint to run the bulk
    /// increment + conditional sum logic required by the interview brief.
    /// </summary>
    /// <remarks>
    /// All endpoints return JSON and are designed to be consumed by the React client.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeeService _service;

        /// <summary>
        /// Creates a new <see cref="EmployeesController"/>.
        /// </summary>
        /// <param name="service">Domain service that encapsulates employee data access and business rules.</param>
        /// <param name="logger">Application logger.</param>
        public EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        /// <returns>A JSON array of employees.</returns>
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var dtoList = await _service.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} employees", dtoList.Count);
            return Ok(dtoList);
        }

        /// <summary>
        /// Retrieves a single employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        /// <returns>The employee if found.</returns>
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
            {
                _logger.LogWarning("Employee {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Employee {Id} retrieved successfully", id);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="createDto">The employee payload.</param>
        /// <returns>The created employee with its generated identifier.</returns>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> PostEmployee([FromBody] EmployeeCreateDto createDto)
        {
            var created = await _service.CreateAsync(createDto);
            _logger.LogInformation("Created new employee {Name} with ID {Id}", created.Name, created.Id);

            return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
        }

        /// <summary>
        /// Replaces mutable fields of an existing employee.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        /// <param name="updateDto">New values for the employee.</param>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] EmployeeUpdateDto updateDto)
        {
            var updated = await _service.UpdateAsync(id, updateDto);
            if (!updated)
                return NotFound();

            _logger.LogInformation("Updated employee {Id}", id);
            return NoContent();
        }

        /// <summary>
        /// Deletes an employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Attempted to delete non-existent employee {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Deleted employee {Id}", id);
            return NoContent();
        }
        /// <summary>
        /// Applies the bulk increment rule (E: +1, G: +10, others: +100) and returns
        /// the sum of values for names starting with A/B/C when the total is ≥ 11171.
        /// </summary>
        /// <returns>HTTP 200 with the sum payload when the threshold is met; otherwise 204.</returns>
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("update-values-and-sum")]
        public async Task<ActionResult<object>> UpdateValuesAndReturnFilteredSum()
        {
            var total = await _service.IncrementValuesAndGetAbcSumAsync();
            _logger.LogInformation("Performed bulk update and sum. Result total: {Total}", total ?? 0);

            if (!total.HasValue)
                return NoContent();

            return Ok(new { SumOfABC = total.Value });
        }
    }
}
