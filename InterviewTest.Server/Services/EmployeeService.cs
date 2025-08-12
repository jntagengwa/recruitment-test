
#nullable enable


using AutoMapper;
using InterviewTest.Server.Data;
using InterviewTest.Server.DTOs;
using InterviewTest.Server.Model;
using Microsoft.EntityFrameworkCore;

namespace InterviewTest.Server.Services
{
    /// <summary>
    /// Defines contract for employee-related data operations, including CRUD and specialized business logic.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Retrieves all employees from the data store as a list of DTOs.
        /// </summary>
        /// <returns>A list of <see cref="EmployeeDto"/> representing all employees.</returns>
        Task<List<EmployeeDto>> GetAllAsync();

        /// <summary>
        /// Retrieves a single employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee.</param>
        /// <returns>
        /// An <see cref="EmployeeDto"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<EmployeeDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new employee in the data store.
        /// </summary>
        /// <param name="dto">The data transfer object containing employee creation data.</param>
        /// <returns>
        /// The created <see cref="EmployeeDto"/> representing the new employee.
        /// </returns>
        Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);

        /// <summary>
        /// Updates an existing employee's information in the data store.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to update.</param>
        /// <param name="dto">The data transfer object containing updated employee data.</param>
        /// <returns>
        /// <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto);

        /// <summary>
        /// Deletes an employee from the data store by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to delete.</param>
        /// <returns>
        /// <c>true</c> if the deletion was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Performs a bulk increment on employee values based on name prefix and returns the sum of values
        /// for employees whose names start with A, B, or C, if that sum meets or exceeds 11171. Otherwise, returns null.
        /// </summary>
        /// <returns>
        /// The sum of values for employees with names starting with A, B, or C if the sum is at least 11171; otherwise, <c>null</c>.
        /// </returns>
        Task<int?> IncrementValuesAndGetAbcSumAsync();
    }

    /// <summary>
    /// Provides concrete implementations for employee-related operations, including CRUD and business logic,
    /// leveraging Entity Framework Core for data access and AutoMapper for DTO mapping.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class with required dependencies.
        /// </summary>
        /// <param name="context">The application's database context for employee data access.</param>
        /// <param name="mapper">The AutoMapper instance for mapping between entities and DTOs.</param>
        /// <param name="logger">The logger instance for logging operational information and errors.</param>
        public EmployeeService(AppDbContext context, IMapper mapper, ILogger<EmployeeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously retrieves all employees from the database and maps them to DTOs.
        /// </summary>
        /// <returns>A list of <see cref="EmployeeDto"/> representing all employees in the system.</returns>
        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            var entities = await _context.Employees.AsNoTracking().ToListAsync();
            return _mapper.Map<List<EmployeeDto>>(entities);
        }

        /// <summary>
        /// Asynchronously retrieves a specific employee by their identifier and maps the result to a DTO.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to retrieve.</param>
        /// <returns>
        /// An <see cref="EmployeeDto"/> if the employee exists; otherwise, <c>null</c>.
        /// </returns>
        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return entity == null ? null : _mapper.Map<EmployeeDto>(entity);
        }

        /// <summary>
        /// Asynchronously creates a new employee in the database from the provided creation DTO.
        /// </summary>
        /// <param name="dto">The <see cref="EmployeeCreateDto"/> containing the employee's creation data.</param>
        /// <returns>
        /// The <see cref="EmployeeDto"/> representing the newly created employee.
        /// </returns>
        public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
        {
            var entity = _mapper.Map<Employee>(dto);
            _context.Employees.Add(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created employee {Name} with Id {Id}", entity.Name, entity.Id);
            return _mapper.Map<EmployeeDto>(entity);
        }

        /// <summary>
        /// Asynchronously updates the specified employee's information in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to update.</param>
        /// <param name="dto">The <see cref="EmployeeUpdateDto"/> containing updated employee data.</param>
        /// <returns>
        /// <c>true</c> if the update was successful; otherwise, <c>false</c> (e.g., if the employee does not exist).
        /// </returns>
        public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var entity = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;

            // Explicitly set fields (clear, predictable)
            entity.Name = dto.Name;
            entity.Value = dto.Value;

            // Ensure changes are tracked (belt-and-braces)
            _context.Entry(entity).Property(e => e.Name).IsModified = true;
            _context.Entry(entity).Property(e => e.Value).IsModified = true;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated employee {Id}", id);
            return true;
        }

        /// <summary>
        /// Asynchronously deletes an employee from the database by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to delete.</param>
        /// <returns>
        /// <c>true</c> if the deletion was successful; otherwise, <c>false</c> (e.g., if the employee does not exist).
        /// </returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;

            _context.Employees.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted employee {Id}", id);
            return true;
        }

        /// <summary>
        /// Performs a bulk increment operation on employee values based on name prefix and computes the sum of values
        /// for employees whose names begin with A, B, or C. If the sum is at least 11171, returns the sum; otherwise, returns null.
        /// </summary>
        /// <returns>
        /// The sum of values for employees with names starting with A, B, or C if the sum is greater than or equal to 11171; otherwise, <c>null</c>.
        /// </returns>
        public async Task<int?> IncrementValuesAndGetAbcSumAsync()
        {
            // Bulk update Values based on first letter of Name
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE Employees
                SET Value = 
                    CASE
                        WHEN Name LIKE 'E%' THEN Value + 1
                        WHEN Name LIKE 'G%' THEN Value + 10
                        ELSE Value + 100
                    END");

            var total = await _context.Employees
                .Where(e => EF.Functions.Like(e.Name, "A%") ||
                            EF.Functions.Like(e.Name, "B%") ||
                            EF.Functions.Like(e.Name, "C%"))
                .SumAsync(e => (int?)e.Value);

            if (total.HasValue && total.Value >= 11171)
            {
                _logger.LogInformation("ABC sum after increment: {Total}", total.Value);
                return total.Value;
            }

            _logger.LogInformation("ABC sum after increment below threshold (value: {Total})", total ?? 0);
            return null;
        }
    }
}