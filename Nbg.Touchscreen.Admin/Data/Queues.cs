namespace Nbg.Touchscreen.Admin.Data
{
    public class Queue
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int? ServiceId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Service? Service { get; set; }
    }

}

