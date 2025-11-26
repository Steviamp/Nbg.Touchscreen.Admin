using Microsoft.EntityFrameworkCore;

namespace Nbg.Touchscreen.Admin.Data.helpers
{
    public class HolidayService
    {
        private readonly AppDbContext _db;
        public HolidayService(AppDbContext db) => _db = db;

        public async Task<bool> IsHolidayAsync(int serviceId, DateOnly date, CancellationToken ct = default)
        {
            // 1) Service override (Open/Closed) — κερδίζει πάντα
            var ov = await _db.ServiceHolidays.AsNoTracking().FirstOrDefaultAsync(h =>
                h.ServiceId == serviceId && h.IsActive &&
                (
                  (!h.IsRecurring && h.Date == date) ||
                  (h.IsRecurring && h.RecurringMonth == date.Month && h.RecurringDay == date.Day)
                ), ct);

            if (ov is not null)
                return ov.OverrideMode == ServiceHolidayOverrideMode.ForceClosed;

            // 2) Global
            var isGlobal = await _db.GlobalHolidays.AsNoTracking().AnyAsync(h =>
                h.IsActive &&
                (
                  (!h.IsRecurring && h.Date == date) ||
                  (h.IsRecurring && h.RecurringMonth == date.Month && h.RecurringDay == date.Day)
                ), ct);

            return isGlobal;
        }

        public async Task<IReadOnlyList<DateOnly>> GetAllForServiceAsync(int serviceId, int year, CancellationToken ct = default)
        {
            var list = new HashSet<DateOnly>();

            var globals = await _db.GlobalHolidays.AsNoTracking().Where(h => h.IsActive).ToListAsync(ct);
            foreach (var g in globals)
            {
                if (g.IsRecurring && g.RecurringMonth.HasValue && g.RecurringDay.HasValue)
                    list.Add(new DateOnly(year, g.RecurringMonth.Value, g.RecurringDay.Value));
                else if (!g.IsRecurring && g.Date.HasValue && g.Date.Value.Year == year)
                    list.Add(g.Date.Value);
            }

            var svc = await _db.ServiceHolidays.AsNoTracking()
                .Where(h => h.ServiceId == serviceId && h.IsActive)
                .ToListAsync(ct);

            foreach (var h in svc)
            {
                DateOnly? d = null;
                if (h.IsRecurring && h.RecurringMonth.HasValue && h.RecurringDay.HasValue)
                    d = new DateOnly(year, h.RecurringMonth.Value, h.RecurringDay.Value);
                else if (!h.IsRecurring && h.Date.HasValue && h.Date.Value.Year == year)
                    d = h.Date.Value;

                if (d is null) continue;

                // αν υπάρχει ForceOpen σε εκείνη τη μέρα, αφαίρεσέ τη (ή μην την προσθέσεις)
                if (h.OverrideMode == ServiceHolidayOverrideMode.ForceOpen)
                    list.Remove(d.Value);
                else
                    list.Add(d.Value); // ForceClosed
            }

            return list.OrderBy(x => x).ToList();
        }
    }

}
