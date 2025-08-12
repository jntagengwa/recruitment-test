

using System.ComponentModel.DataAnnotations;

namespace InterviewTest.Server.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating existing employee records.
    /// Used in update operations to modify employee details while enforcing validation constraints.
    /// </summary>
    public class EmployeeUpdateDto
    {
        /// <summary>
        /// The updated name of the employee.
        /// Must be provided and cannot exceed 100 characters.
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The updated value associated with the employee.
        /// Must be a non-negative integer.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Value must be a non-negative integer.")]
        public int Value { get; set; }
    }
}