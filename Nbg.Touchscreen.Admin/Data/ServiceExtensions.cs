using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Nbg.Touchscreen.Admin.Data;

public static class ServiceExtensions
{
    private static HashSet<DateOnly> ParseBankHolidays(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new HashSet<DateOnly>();

        try
        {
            var list = JsonSerializer.Deserialize<List<DateOnly>>(json)
                       ?? new List<DateOnly>();
            return new HashSet<DateOnly>(list);
        }
        catch
        {
            return new HashSet<DateOnly>();
        }
    }

    /// <summary>
    /// True αν η συγκεκριμένη ημερομηνία είναι αργία για το Service.
    /// </summary>
    public static bool IsBankHolidayOn(this Service service, DateTime localDateTime)
    {
        if (service == null) return false;

        var today = DateOnly.FromDateTime(localDateTime);
        var holidays = ParseBankHolidays(service.BankHolidaysJson);
        return holidays.Contains(today);
    }

    /// <summary>
    /// True αν το Service μπορεί να εκδίδει εισιτήρια την συγκεκριμένη local ημερομηνία/ώρα.
    /// </summary>
    public static bool CanIssueTicketAt(this Service service, DateTime localDateTime)
    {
        if (service == null) return false;

        // 1. Αν είναι αργία -> δεν κόβει εισιτήριο
        if (service.IsBankHolidayOn(localDateTime))
            return false;

        var time = localDateTime.TimeOfDay;

        bool enabled;
        TimeSpan? from;
        TimeSpan? to;

        switch (localDateTime.DayOfWeek)
        {
            case DayOfWeek.Monday:
                enabled = service.MondayEnabled;
                from = service.MondayFrom;
                to = service.MondayTo;
                break;
            case DayOfWeek.Tuesday:
                enabled = service.TuesdayEnabled;
                from = service.TuesdayFrom;
                to = service.TuesdayTo;
                break;
            case DayOfWeek.Wednesday:
                enabled = service.WednesdayEnabled;
                from = service.WednesdayFrom;
                to = service.WednesdayTo;
                break;
            case DayOfWeek.Thursday:
                enabled = service.ThursdayEnabled;
                from = service.ThursdayFrom;
                to = service.ThursdayTo;
                break;
            case DayOfWeek.Friday:
                enabled = service.FridayEnabled;
                from = service.FridayFrom;
                to = service.FridayTo;
                break;
            case DayOfWeek.Saturday:
                enabled = service.SaturdayEnabled;
                from = service.SaturdayFrom;
                to = service.SaturdayTo;
                break;
            case DayOfWeek.Sunday:
                enabled = service.SundayEnabled;
                from = service.SundayFrom;
                to = service.SundayTo;
                break;
            default:
                return false;
        }

        if (!enabled || from is null || to is null)
            return false;

        return time >= from.Value && time <= to.Value;
    }
}
