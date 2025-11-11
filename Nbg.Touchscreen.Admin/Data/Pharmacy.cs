namespace Nbg.Touchscreen.Admin.Data
{
    public class Pharmacy
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
