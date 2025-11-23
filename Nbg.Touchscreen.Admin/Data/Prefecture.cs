namespace Nbg.Touchscreen.Admin.Data
{
    public class Prefecture
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string NameEl { get; set; } = "";
        public string? NameEn { get; set; }
        public bool IsActive { get; set; } = true;
    }
}