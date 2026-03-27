using Microsoft.AspNetCore.Identity;

namespace webfoodprime.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Wallet Wallet { get; set; }
        public virtual Cart Cart { get; set; }

    }
}
