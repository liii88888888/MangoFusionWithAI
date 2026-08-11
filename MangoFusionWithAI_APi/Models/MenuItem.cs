using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusionWithAI_APi.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; } = string.Empty;
        [Range(1,1000)]
        public double Price { get; set; }
        public string? Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SpecialTag { get; set; }
        //[Required]
        public string? Image { get; set; } = string.Empty;
        [NotMapped]
        public double Rating { get; set; }

    }
}


