using Microsoft.AspNetCore.Identity;

namespace Nbg.Touchscreen.Admin.Data
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
    }
}
