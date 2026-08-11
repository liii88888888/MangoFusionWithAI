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
    [Authorize]
    [ApiController]
    public class OrderHeaderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ApiResponse _response;

        public OrderHeaderController(IOrderService orderService)
        {
            _orderService = orderService;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetOrders(string userId = "")
        {
            bool isAdmin = User.IsInRole(SD.Role_Admin);
            var orders = await _orderService.GetOrdersAsync(userId, isAdmin);
            _response.Result = orders;
            _response.StatusCode=System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<ApiResponse>> GetOrder(int orderId)
        {
            if (orderId == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Id异常");
                return BadRequest(_response);
            }

            var orderHeader = await _orderService.GetOrderByIdAsync(orderId);
            
            if (orderHeader is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("订单不存在");
                return NotFound(_response);

            }

            _response.Result = orderHeader;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess=false;
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                       .SelectMany(u => u.Errors)
                       .Select(u => u.ErrorMessage)
                       .ToList();
                    return BadRequest(_response);
                }
                var orderHeader= await _orderService.CreateOrderAsync(orderHeaderDTO);
                _response.Result = orderHeader;
                _response.IsSuccess = true;
                _response.StatusCode= HttpStatusCode.OK;
                return Ok(_response) ;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [HttpPut("{orderId:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrder(int orderId,[FromBody] OrderHeaderUpdateDTO orderHeaderDTO)
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

                if (orderId !=orderHeaderDTO.OrderHeaderId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.AddRange("Id 与 目标修改对象Id不符合");
                    return BadRequest(_response);
                }

                var (succeeded, error) = await _orderService.UpdateOrderAsync(orderId, orderHeaderDTO);
                
                if (!succeeded)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add(error);
                    return NotFound(_response);
                }

                _response.IsSuccess = true;
                _response.StatusCode=HttpStatusCode.NoContent;
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
