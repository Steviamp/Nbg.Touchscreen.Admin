namespace Nbg.Touchscreen.Admin.Data
{
    public enum ServiceHolidayOverrideMode : byte { ForceClosed = 0, ForceOpen = 1 }

    public class GlobalHoliday
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateOnly? Date { get; set; }          // one-off
        public bool IsRecurring { get; set; }        // yearly
        public byte? RecurringMonth { get; set; }    // 1..12
        public byte? RecurringDay { get; set; }      // 1..31
        public bool IsActive { get; set; } = true;
    }

    public class ServiceHoliday
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;
        public string? Name { get; set; }
        public DateOnly? Date { get; set; }          // one-off
        public bool IsRecurring { get; set; }        // yearly
        public byte? RecurringMonth { get; set; }
        public byte? RecurringDay { get; set; }
        public ServiceHolidayOverrideMode OverrideMode { get; set; } = ServiceHolidayOverrideMode.ForceClosed;
        public bool IsActive { get; set; } = true;
    }

}
