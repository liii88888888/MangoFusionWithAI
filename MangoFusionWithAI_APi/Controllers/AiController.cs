using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services.IServices;
using MangoFusionWithAI_APi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MangoFusionWithAI_APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly ApiResponse _response;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
            _response = new ApiResponse();
        }

        /// <summary>
        /// [管理端] AI 一键生成菜品营销描述文案
        /// 要求 Admin 角色
        /// </summary>
        [HttpPost("generate-description")]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> GenerateDescription(
            [FromBody] AiGenerateDescriptionRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(_response);
                }

                var result = await _aiService.GenerateDescriptionAsync(request);
                _response.Result = result;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (InvalidOperationException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.ServiceUnavailable;
                _response.ErrorMessages = [ex.Message];
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = [$"AI 服务异常：{ex.Message}"];
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        /// <summary>
        /// [用户端] AI 自然语言搜索菜品
        /// 无需登录即可使用
        /// </summary>
        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<IActionResult> NaturalLanguageSearch(
            [FromBody] AiSearchRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(_response);
                }

                var result = await _aiService.NaturalLanguageSearchAsync(request.Query);

                // 如果 AI 提取不到关键词（非食物相关输入），返回空结果
                if (result.Keywords.Count == 0)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = result;
                    _response.ErrorMessages = ["未能从输入中提取到有效的美食关键词，请尝试更具体地描述你想吃什么。"];
                    return Ok(_response);
                }

                _response.Result = result;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (InvalidOperationException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.ServiceUnavailable;
                _response.ErrorMessages = [ex.Message];
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = [$"AI 搜索异常：{ex.Message}"];
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        /// <summary>
        /// [管理端] 将 AI 生成的描述写入指定菜品
        /// 要求 Admin 角色
        /// </summary>
        [HttpPatch("apply-description")]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> ApplyDescription(
            [FromBody] ApplyDescriptionRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(_response);
                }

                await _aiService.ApplyDescriptionAsync(request.MenuItemId, request.Description);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = [ex.Message];
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = [$"应用描述失败：{ex.Message}"];
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }
    }
}
