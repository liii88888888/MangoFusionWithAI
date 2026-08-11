using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MangoFusionWithAI_APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApiResponse _response;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _response = new ApiResponse();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                foreach (var error in ModelState.Values)
                {
                    foreach (var item in error.Errors)
                    {
                        _response.ErrorMessages.Add(item.ErrorMessage);
                    }
                }
                return BadRequest(_response);
            }

            var (succeed,errors) = await _authService.RegisterAsync(model);
            if (succeed)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.AddRange(errors);
            return BadRequest(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                foreach (var error in ModelState.Values)
                {
                    foreach (var item in error.Errors)
                    {
                        _response.ErrorMessages.Add(item.ErrorMessage);
                    }
                }
                return BadRequest(_response);
            }

            var (succeed, result,errors) = await _authService.LoginAsync(model);
            if (succeed && result!=null)
            {
                _response.Result = result;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            _response.Result = new LoginResponseDTO();
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.AddRange(errors);
            return BadRequest(_response);

        }



    }
}

