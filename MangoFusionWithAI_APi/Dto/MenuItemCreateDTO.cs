using System.ComponentModel.DataAnnotations;

namespace MangoFusionWithAI_APi.Dto
{
    public class MenuItemCreateDTO
    {
        [Required]
        public string? Name { get; set; } = string.Empty;
        [Range(1, 1000)]
        public double Price { get; set; }
        public string? Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SpecialTag { get; set; }
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
