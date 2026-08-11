using System.ComponentModel.DataAnnotations;

namespace MangoFusionWithAI_APi.Dto
{
    /// <summary>
    /// AI 自然语言搜索请求
    /// </summary>
    public class AiSearchRequestDTO
    {
        [Required]
        [MinLength(2)]
        public string Query { get; set; } = string.Empty;
    }
}
