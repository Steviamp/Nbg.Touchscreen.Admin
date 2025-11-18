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
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
        public string? PharmacyIP { get; set; }
        public int? Port { get; set; }
        public string? WorkingHoursJson { get; set; }
        public string? TicketingRulesJson { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
