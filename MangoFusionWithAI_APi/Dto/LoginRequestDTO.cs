using System.ComponentModel.DataAnnotations;

namespace MangoFusionWithAI_APi.Dto
{
    public class LoginRequestDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
