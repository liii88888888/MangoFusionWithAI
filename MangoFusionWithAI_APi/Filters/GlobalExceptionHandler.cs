using MangoFusionWithAI_APi.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace MangoFusionWithAI_APi.Filters
{
    /// <summary>
    /// 全局异常处理器（.NET 8+ IExceptionHandler）
    /// 把所有未捕获异常统一翻译成 ApiResponse 格式返回，
    /// 避免 Controller 里重复写 try/catch，也避免前端拿到格式不一的错误响应。
    /// 对应 Java 版的 GlobalExceptionHandler（@RestControllerAdvice）。
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // 记录完整堆栈，方便排错
            _logger.LogError(exception, "未处理异常：{Message}", exception.Message);

            var response = new ApiResponse
            {
                IsSuccess = false
            };

            switch (exception)
            {
                // 资源不存在 → 404（对应 Service 层 throw new KeyNotFoundException）
                case KeyNotFoundException notFound:
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages = [notFound.Message];
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                // 其余一切异常 → 500
                default:
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.ErrorMessages = ["服务器内部错误：" + exception.Message];
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true; // 已处理，异常不再继续上抛
        }
    }
}
