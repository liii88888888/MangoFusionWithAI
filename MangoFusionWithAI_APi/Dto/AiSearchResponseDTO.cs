using MangoFusionWithAI_APi.Models;

namespace MangoFusionWithAI_APi.Dto
{
    /// <summary>
    /// AI 自然语言搜索响应
    /// </summary>
    public class AiSearchResponseDTO
    {
        /// <summary>
        /// AI 提取的搜索关键词
        /// </summary>
        public List<string> Keywords { get; set; } = [];

        /// <summary>
        /// 匹配的菜品列表
        /// </summary>
        public List<MenuItem> MenuItems { get; set; } = [];

        /// <summary>
        /// 用户原始输入
        /// </summary>
        public string OriginalQuery { get; set; } = string.Empty;
    }
}
