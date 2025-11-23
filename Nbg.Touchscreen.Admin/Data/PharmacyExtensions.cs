using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Nbg.Touchscreen.Admin.Data;

public static class PharmacyExtensions
{
    /// <summary>
    /// Επιστρέφει true αν το φαρμακείο θεωρείται "ανοιχτό"
    /// τη δεδομένη local ημερομηνία/ώρα.
    /// </summary>
    public static bool IsOpenAt(this Pharmacy pharmacy, DateTime localDateTime)
    {
        if (pharmacy == null)
            return false;

        // 1. Αν είναι αργία -> κλειστό
        var today = DateOnly.FromDateTime(localDateTime);

        var bankHolidays = ParseBankHolidays(pharmacy.BankHolidaysJson);
        if (bankHolidays.Contains(today))
            return false;

        // 2. Βρες το αντίστοιχο ωράριο μέρας
        var time = localDateTime.TimeOfDay;

        bool enabled;
        TimeSpan? from;
        TimeSpan? to;

        switch (localDateTime.DayOfWeek)
        {
            case DayOfWeek.Monday:
                enabled = pharmacy.MondayEnabled;
                from = pharmacy.MondayFrom;
                to = pharmacy.MondayTo;
                break;
            case DayOfWeek.Tuesday:
                enabled = pharmacy.TuesdayEnabled;
                from = pharmacy.TuesdayFrom;
                to = pharmacy.TuesdayTo;
                break;
            case DayOfWeek.Wednesday:
                enabled = pharmacy.WednesdayEnabled;
                from = pharmacy.WednesdayFrom;
                to = pharmacy.WednesdayTo;
                break;
            case DayOfWeek.Thursday:
                enabled = pharmacy.ThursdayEnabled;
                from = pharmacy.ThursdayFrom;
                to = pharmacy.ThursdayTo;
                break;
            case DayOfWeek.Friday:
                enabled = pharmacy.FridayEnabled;
                from = pharmacy.FridayFrom;
                to = pharmacy.FridayTo;
                break;
            case DayOfWeek.Saturday:
                enabled = pharmacy.SaturdayEnabled;
                from = pharmacy.SaturdayFrom;
                to = pharmacy.SaturdayTo;
                break;
            case DayOfWeek.Sunday:
                enabled = pharmacy.SundayEnabled;
                from = pharmacy.SundayFrom;
                to = pharmacy.SundayTo;
                break;
            default:
                return false;
        }

        if (!enabled || from is null || to is null)
            return false;

        // 3. Έλεγχος ώρας
        return time >= from.Value && time <= to.Value;
    }

    private static HashSet<DateOnly> ParseBankHolidays(string? bankHolidaysJson)
    {
        if (string.IsNullOrWhiteSpace(bankHolidaysJson))
            return new HashSet<DateOnly>();

        try
        {
            var list = JsonSerializer.Deserialize<List<DateOnly>>(bankHolidaysJson)
                       ?? new List<DateOnly>();
            return new HashSet<DateOnly>(list);
        }
        catch
        {
            // Αν για κάποιο λόγο χαλάσει το JSON, απλά μη θεωρείς καμία αργία
            return new HashSet<DateOnly>();
        }
    }
}

