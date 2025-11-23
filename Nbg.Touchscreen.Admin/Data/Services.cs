namespace Nbg.Touchscreen.Admin.Data
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int QueueId { get; set; }
        public string Ip { get; set; } = default!;
        public int Port { get; set; }

        public bool IsActive { get; set; }
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

        public ICollection<Queue> Queues { get; set; } = new List<Queue>();
    }
}
