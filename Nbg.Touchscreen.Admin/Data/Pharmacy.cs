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
        public DateTime CreatedAtUtc { get; set; }
        public bool MondayEnabled { get; set; }
        public TimeSpan? MondayFrom { get; set; }
        public TimeSpan? MondayTo { get; set; }

        public bool TuesdayEnabled { get; set; }
        public TimeSpan? TuesdayFrom { get; set; }
        public TimeSpan? TuesdayTo { get; set; }

        public bool WednesdayEnabled { get; set; }
        public TimeSpan? WednesdayFrom { get; set; }
        public TimeSpan? WednesdayTo { get; set; }

        public bool ThursdayEnabled { get; set; }
        public TimeSpan? ThursdayFrom { get; set; }
        public TimeSpan? ThursdayTo { get; set; }

        public bool FridayEnabled { get; set; }
        public TimeSpan? FridayFrom { get; set; }
        public TimeSpan? FridayTo { get; set; }

        public bool SaturdayEnabled { get; set; }
        public TimeSpan? SaturdayFrom { get; set; }
        public TimeSpan? SaturdayTo { get; set; }

        public bool SundayEnabled { get; set; }
        public TimeSpan? SundayFrom { get; set; }
        public TimeSpan? SundayTo { get; set; }
        public string? BankHolidaysJson { get; set; }
        public int? PrefectureId { get; set; }
        public Prefecture? Prefecture { get; set; }
    }
}
