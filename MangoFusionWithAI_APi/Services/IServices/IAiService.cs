using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;

namespace MangoFusionWithAI_APi.Services.IServices
{
    /// <summary>
    /// AI 服务接口，封装大模型调用与智能搜索逻辑
    /// </summary>
    public interface IAiService
    {
        /// <summary>
        /// 根据菜品名称和价格，调用 DeepSeek 生成营销描述文案
        /// </summary>
        /// <param name="request">包含菜品名称、价格、分类等信息的请求</param>
        /// <returns>AI 生成的营销文案</returns>
        Task<AiGenerateDescriptionResponseDTO> GenerateDescriptionAsync(AiGenerateDescriptionRequestDTO request);

        /// <summary>
        /// 从用户自然语言查询中提取搜索关键词，并在数据库中模糊匹配菜品
        /// </summary>
        /// <param name="query">用户自然语言查询（如"我想吃辣的便宜的菜"）</param>
        /// <returns>提取的关键词 + 匹配到的菜品列表</returns>
        Task<AiSearchResponseDTO> NaturalLanguageSearchAsync(string query);

        /// <summary>
        /// 将 AI 生成的描述文案写入指定菜品
        /// </summary>
        /// <param name="menuItemId">菜品ID</param>
        /// <param name="description">AI 生成的描述文案</param>
        Task ApplyDescriptionAsync(int menuItemId, string description);
    }
}
