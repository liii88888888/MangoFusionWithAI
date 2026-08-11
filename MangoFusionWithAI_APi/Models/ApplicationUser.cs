using Microsoft.AspNetCore.Identity;

namespace MangoFusionWithAI_APi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
