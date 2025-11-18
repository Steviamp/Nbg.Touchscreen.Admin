namespace Nbg.Touchscreen.Admin.Data
{
    public class Queue
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public int? ServiceId { get; set; } 
        public Service? Service { get; set; } 

        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
