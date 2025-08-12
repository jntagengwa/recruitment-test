using System.ComponentModel.DataAnnotations;
namespace InterviewTest.Server.DTOs
{
    /// <summary>
    /// Data Transfer Object used for creating a new employee record.
    /// Contains the basic details required to add an employee to the system.
    /// </summary>
    public class EmployeeCreateDto
    {
        /// <summary>
        /// The full name of the employee.
        /// Required. Maximum length of 100 characters.
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// A numeric value associated with the employee.
        /// Must be a non-negative integer.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Value must be a non-negative integer.")]
        public int Value { get; set; }
    }
}
