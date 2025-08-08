namespace InterviewTest.Server.Model
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Value { get; set; }
    }
}
