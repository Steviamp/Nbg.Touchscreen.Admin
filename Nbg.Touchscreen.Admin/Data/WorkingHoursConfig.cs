namespace Nbg.Touchscreen.Admin.Data;

public class WorkingHoursConfig
{
    public List<DaySchedule> Days { get; set; } = new();
    public List<DateOnly> BankHolidays { get; set; } = new();
}

public class DaySchedule
{
    public DayOfWeek Day { get; set; }
    public bool Enabled { get; set; }
    public TimeSpan From { get; set; } = new TimeSpan(9, 0, 0);
    public TimeSpan To { get; set; } = new TimeSpan(17, 0, 0);
}
