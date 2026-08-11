using System.ComponentModel.DataAnnotations;

namespace MangoFusionWithAI_APi.Dto
{
    /// <summary>
    /// 应用 AI 生成的描述到菜品
    /// </summary>
    public class ApplyDescriptionRequestDTO
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        [MinLength(1)]
        public string Description { get; set; } = string.Empty;
    }
}
