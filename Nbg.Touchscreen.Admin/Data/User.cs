using System.ComponentModel.DataAnnotations;

namespace Nbg.Touchscreen.Admin.Data
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(256)]
        public string Email { get; set; } = default!;

        [Required, MaxLength(200)]
        public string PasswordPlain { get; set; } = default!; 

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Viewer";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
