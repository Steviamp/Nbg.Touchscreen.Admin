namespace Nbg.Touchscreen.Admin.Data
{
    public class Pharmacy
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? OwnerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public double? Latitude { get; set; }   // 23.68756
        public double? Longitude { get; set; }  // 37.94736
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
