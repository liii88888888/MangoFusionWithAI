using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services;
using MangoFusionWithAI_APi.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MangoFusionWithAI_APi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ApiResponse _response;

        public OrderDetailsController(IOrderService orderService)
        {
            _orderService = orderService;
            _response = new ApiResponse();
        }

        [HttpPut("{orderDetailsId:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrder(int orderDetailsId, [FromBody] OrderDetailsUpdateDTO orderDetailsDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                       .SelectMany(u => u.Errors)
                       .Select(u => u.ErrorMessage)
                       .ToList();
                    return BadRequest(_response);
                }

                if (orderDetailsId != orderDetailsDTO.OrderDetailId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ["Id 与 目标修改对象Id不符合"];
                    return BadRequest(_response);
                }

                var (succeeded, error) = await _orderService.UpdateRatingAsync(orderDetailsId, orderDetailsDTO.Rating);

                if (!succeeded)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add(error);
                    return NotFound(_response);
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }
    }
}
