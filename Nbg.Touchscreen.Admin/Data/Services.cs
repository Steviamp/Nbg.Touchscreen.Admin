namespace Nbg.Touchscreen.Admin.Data
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Ip { get; set; } = default!;
        public int Port { get; set; }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Queue> Queues { get; set; } = new List<Queue>();
    }

}

