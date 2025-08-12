namespace InterviewTest.Server.DTOs
{
    /// <summary>
    /// Data transfer object for transferring employee details between layers.
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the employee.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value or metric associated with the employee.
        /// </summary>
        public int Value { get; set; }
    }
}
