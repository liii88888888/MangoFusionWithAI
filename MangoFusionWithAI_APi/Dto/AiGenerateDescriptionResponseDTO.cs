namespace MangoFusionWithAI_APi.Dto
{
    /// <summary>
    /// AI 营销描述生成响应
    /// </summary>
    public class AiGenerateDescriptionResponseDTO
    {
        /// <summary>
        /// AI 生成的营销描述文案
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// AI 模型名称（如 deepseek-chat）
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Token 使用量
        /// </summary>
        public int TotalTokens { get; set; }
    }
}
