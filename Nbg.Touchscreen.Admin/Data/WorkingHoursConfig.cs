using System;
using System.Collections.Generic;

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
    public TimeSpan? From { get; set; }
    public TimeSpan? To { get; set; }
}
