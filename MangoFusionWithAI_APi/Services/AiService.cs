using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MangoFusionWithAI_APi.Data;
using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace MangoFusionWithAI_APi.Services
{
    /// <summary>
    /// AI 服务实现：调用 DeepSeek 大模型，实现营销文案生成和自然语言搜索
    /// </summary>
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AiService> _logger;

        private const string DEEPSEEK_MODEL = "deepseek-chat";
        private const double CREATIVE_TEMPERATURE = 0.8;  // 营销文案需要创意
        private const double PRECISE_TEMPERATURE = 0.1;   // 关键词提取需要精确

        public AiService(
            HttpClient httpClient,
            ApplicationDbContext db,
            IConfiguration configuration,
            ILogger<AiService> logger)
        {
            _httpClient = httpClient;
            _db = db;
            _configuration = configuration;
            _logger = logger;
        }

        // ============================
        //  1. 营销描述生成
        // ============================

        public async Task<AiGenerateDescriptionResponseDTO> GenerateDescriptionAsync(
            AiGenerateDescriptionRequestDTO request)
        {
            _logger.LogInformation("开始生成菜品营销描述：{Name}，价格：¥{Price}",
                request.Name, request.Price);

            var systemPrompt = BuildMarketingSystemPrompt();
            var userPrompt = BuildMarketingUserPrompt(request);

            var (content, totalTokens) = await CallDeepSeekAsync(
                systemPrompt, userPrompt, CREATIVE_TEMPERATURE, 300);

            return new AiGenerateDescriptionResponseDTO
            {
                Description = content,
                Model = DEEPSEEK_MODEL,
                TotalTokens = totalTokens
            };
        }

        /// <summary>
        /// 构建营销专家 System Prompt
        /// </summary>
        private static string BuildMarketingSystemPrompt()
        {
            return """
                   你是一个顶级餐饮营销文案专家，拥有 10 年高端餐厅菜单设计经验。
                   你的任务是为菜品撰写一段诱人的营销描述文案。

                   请严格遵循以下规则：
                   1. 字数控制在 50-80 字之间
                   2. 使用感官化语言（视觉、味觉、嗅觉），让读者产生食欲
                   3. 突出菜品的独特卖点和特色风味
                   4. 语气专业但不做作，像米其林菜单的风格
                   5. 必须使用中文输出
                   6. 不使用虚假宣传词汇（如"第一"、"最好"、"世界级"）
                   7. 输出纯文案文本，不要加任何前缀、引号或 markdown 标记
                   8. 如果用户提供了口感风格提示，请据此调整文案风格
                   """;
        }

        /// <summary>
        /// 构建营销文案 User Prompt
        /// </summary>
        private static string BuildMarketingUserPrompt(AiGenerateDescriptionRequestDTO request)
        {
            var flavorHint = string.IsNullOrWhiteSpace(request.FlavorStyle)
                ? ""
                : $"\n菜品口感风格：{request.FlavorStyle}";
            var categoryHint = string.IsNullOrWhiteSpace(request.Category)
                ? ""
                : $"\n菜品分类：{request.Category}";

            return $"""
                    请为以下菜品生成营销描述：

                    菜品名称：{request.Name}
                    价格：¥{request.Price:F2}{categoryHint}{flavorHint}

                    请直接输出文案：
                    """;
        }

        // ============================
        //  2. 自然语言搜索
        // ============================

        public async Task<AiSearchResponseDTO> NaturalLanguageSearchAsync(string query)
        {
            _logger.LogInformation("AI 自然语言搜索：{Query}", query);

            // Step 1: AI 提取关键词 + 推荐分类
            var (keywords, suggestedCategory) = await ExtractKeywordsAndCategoryAsync(query);

            _logger.LogInformation("AI 提取关键词：{Keywords}，推荐分类：{Category}",
                string.Join(", ", keywords), suggestedCategory);

            // Step 2: 数据库模糊匹配（优先关键词，回退分类）
            var menuItems = await FuzzySearchMenuItemsAsync(keywords, suggestedCategory);

            _logger.LogInformation("模糊匹配结果数：{Count}", menuItems.Count);

            return new AiSearchResponseDTO
            {
                Keywords = keywords,
                MenuItems = menuItems,
                OriginalQuery = query
            };
        }

        /// <summary>
        /// 调用 DeepSeek 从自然语言中提取搜索关键词 + 推荐分类
        /// </summary>
        private async Task<(List<string> Keywords, string? Category)> ExtractKeywordsAndCategoryAsync(string query)
        {
            // 获取数据库中实际存在的所有分类
            var existingCategories = await _db.MenuItems
                .Select(m => m.Category)
                .Distinct()
                .ToListAsync();
            var categoryList = string.Join("、", existingCategories.Where(c => !string.IsNullOrEmpty(c)));

            var systemPrompt = $"""
                        你是一个美食搜索助手，服务于一家中餐厅的点餐系统。

                        餐厅有以下菜品分类：{categoryList}

                        你的任务是从用户输入中提取搜索关键词。请遵循以下规则：
                        1. 提取与美食、口味、食材、烹饪方式相关的关键词
                        2. 忽略语气词（如"我想"、"有没有"、"帮我找"）
                        3. 每个关键词不超过 4 个字
                        4. 最多提取 5 个关键词
                        5. 输出格式：关键词1,关键词2,...|推荐分类
                           - 如果用户的描述明显属于某个分类（如"甜的"→甜品），请在|后写上该分类
                           - 如果无法判断分类，|后写"无"
                        6. 直接输出结果，不要任何解释或前缀

                        示例：
                        输入："想吃麻辣的重口味的菜"
                        输出：麻辣,重口味,辣,川菜|主菜

                        输入："有没有清淡的适合小孩吃的"
                        输出：清淡,健康,适合小孩|开胃小菜

                        输入："来个甜的饭后吃"
                        输出：甜,饭后,甜品|甜品

                        输入："你好"
                        输出：无|无
                        """;

            var (content, _) = await CallDeepSeekAsync(
                systemPrompt, query, PRECISE_TEMPERATURE, 100);

            return ParseKeywordsAndCategory(content);
        }

        /// <summary>
        /// 解析 AI 返回的 "关键词|分类" 格式
        /// </summary>
        private static (List<string> Keywords, string? Category) ParseKeywordsAndCategory(string aiOutput)
        {
            if (string.IsNullOrWhiteSpace(aiOutput) || aiOutput.Trim() == "无" || aiOutput.Trim() == "无|无")
                return ([], null);

            var parts = aiOutput.Split('|', 2, StringSplitOptions.TrimEntries);

            // 解析关键词
            var keywords = parts[0]
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => k.Length > 0 && k != "无")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(8)
                .ToList();

            // 解析推荐分类
            string? category = null;
            if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) && parts[1] != "无")
                category = parts[1].Trim();

            return (keywords, category);
        }

        /// <summary>
        /// 根据关键词在数据库中模糊匹配菜品
        /// 策略：关键词命中 → 分类命中 → 返回全部（兜底）
        /// </summary>
        private async Task<List<MenuItem>> FuzzySearchMenuItemsAsync(
            List<string> keywords, string? suggestedCategory)
        {
            if (keywords.Count == 0 && string.IsNullOrWhiteSpace(suggestedCategory))
                return [];

            IQueryable<MenuItem> query = _db.MenuItems;
            var result = new List<MenuItem>();
            var seenIds = new HashSet<int>();

            // Phase 1: 关键词多字段模糊匹配
            if (keywords.Count > 0)
            {
                foreach (var keyword in keywords)
                {
                    var matches = await query
                        .Where(m =>
                            (m.Name != null && m.Name.Contains(keyword)) ||
                            (m.Category != null && m.Category.Contains(keyword)) ||
                            (m.Description != null && m.Description.Contains(keyword)) ||
                            (m.SpecialTag != null && m.SpecialTag.Contains(keyword)))
                        .ToListAsync();

                    foreach (var item in matches)
                    {
                        if (seenIds.Add(item.Id))
                            result.Add(item);
                    }
                }
            }

            // Phase 2: 关键词没命中 → 尝试分类匹配
            if (result.Count == 0 && !string.IsNullOrWhiteSpace(suggestedCategory))
            {
                _logger.LogInformation("关键词未命中，尝试分类匹配：{Category}", suggestedCategory);
                var categoryMatches = await query
                    .Where(m => m.Category != null && m.Category.Contains(suggestedCategory))
                    .ToListAsync();

                foreach (var item in categoryMatches)
                {
                    if (seenIds.Add(item.Id))
                        result.Add(item);
                }
            }

            // Phase 3: 仍然没有结果 → 返回全部菜品（兜底）
            if (result.Count == 0 && keywords.Count > 0)
            {
                _logger.LogInformation("分类也未命中，返回全部菜品作为兜底");
                var allItems = await query.ToListAsync();
                result.AddRange(allItems);
            }

            return result;
        }

        // ============================
        //  DeepSeek API 核心调用
        // ============================

        /// <summary>
        /// 调用 DeepSeek Chat API
        /// </summary>
        /// <param name="systemPrompt">系统提示词</param>
        /// <param name="userPrompt">用户提示词</param>
        /// <param name="temperature">温度参数（0-2），越高越有创意</param>
        /// <param name="maxTokens">最大生成 token 数</param>
        /// <returns>(AI 回复内容, 总 token 消耗)</returns>
        private async Task<(string Content, int TotalTokens)> CallDeepSeekAsync(
            string systemPrompt,
            string userPrompt,
            double temperature,
            int maxTokens)
        {
            var apiKey = _configuration["DeepSeek:ApiKey"]
                ?? throw new InvalidOperationException("DeepSeek:ApiKey 未在 appsettings.json 中配置");
            var baseUrl = _configuration["DeepSeek:BaseUrl"] ?? "https://api.deepseek.com/v1";

            var requestBody = new
            {
                model = DEEPSEEK_MODEL,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature,
                max_tokens = maxTokens,
                stream = false
            };

            var opts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var jsonContent = JsonSerializer.Serialize(requestBody, opts);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = httpContent;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var deepSeekResponse = JsonSerializer.Deserialize<DeepSeekChatResponse>(
                responseBody, opts);

            if (deepSeekResponse?.Choices == null || deepSeekResponse.Choices.Count == 0)
                throw new InvalidOperationException("DeepSeek 返回了空响应");

            var content = deepSeekResponse.Choices[0].Message.Content.Trim();
            var totalTokens = deepSeekResponse.Usage?.TotalTokens ?? 0;

            _logger.LogInformation("DeepSeek 调用成功，消耗 Token：{Tokens}", totalTokens);

            return (content, totalTokens);
        }

        // ============================
        //  3. 应用 AI 描述到菜品
        // ============================

        public async Task ApplyDescriptionAsync(int menuItemId, string description)
        {
            var menuItem = await _db.MenuItems.FindAsync(menuItemId)
                ?? throw new KeyNotFoundException($"菜品 ID {menuItemId} 不存在");

            menuItem.Description = description;
            await _db.SaveChangesAsync();

            _logger.LogInformation("AI 描述已应用到菜品 {Name} (ID={Id})", menuItem.Name, menuItemId);
        }
    }

    // ============================
    //  JSON 序列化模型
    // ============================

    /// <summary>
    /// DeepSeek Chat Completion 响应模型（仅映射用到的字段）
    /// </summary>
    internal class DeepSeekChatResponse
    {
        [JsonPropertyName("choices")]
        public List<DeepSeekChoice> Choices { get; set; } = [];

        [JsonPropertyName("usage")]
        public DeepSeekUsage? Usage { get; set; }
    }

    internal class DeepSeekChoice
    {
        [JsonPropertyName("message")]
        public DeepSeekMessage Message { get; set; } = new();
    }

    internal class DeepSeekMessage
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    internal class DeepSeekUsage
    {
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
