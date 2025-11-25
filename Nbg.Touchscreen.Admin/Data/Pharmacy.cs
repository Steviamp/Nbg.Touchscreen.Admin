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
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public string? PharmacyIP { get; set; }
        public int? Port { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int? PrefectureId { get; set; }
        public Prefecture? Prefecture { get; set; }
        public TicketingProvider TicketingProvider { get; set; } = TicketingProvider.None;
    }
}
