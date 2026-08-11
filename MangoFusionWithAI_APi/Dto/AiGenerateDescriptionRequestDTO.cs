using System.ComponentModel.DataAnnotations;

namespace MangoFusionWithAI_APi.Dto
{
    /// <summary>
    /// AI 营销描述生成请求
    /// </summary>
    public class AiGenerateDescriptionRequestDTO
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }

        /// <summary>
        /// 可选：菜品分类，用于生成更准确的描述
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 可选：菜品口感风格提示，如"麻辣"、"清淡"、"酸甜"
        /// </summary>
        public string? FlavorStyle { get; set; }
    }
}
