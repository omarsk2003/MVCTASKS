namespace TASK6.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string phoneNumber { get; set; } = string.Empty;
        public string? EmailAddress { get; set; }
    }
}
