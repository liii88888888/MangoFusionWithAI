using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Net;

namespace MangoFusionWithAI_APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ApiResponse _response;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
            _response = new ApiResponse();
        }

        //查全部菜品
        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems = await _menuItemService.GetAllMenuItemAsync();
            _response.Result = menuItems;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        //查单个菜品
        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            if (id == 0) return BadRequest();

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);

            if (menuItem is null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = menuItem;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        //添加菜品
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = ["File is required"];
                    return BadRequest(_response);
                }

                var menuItem = await _menuItemService.CreateMenuItemAsync(menuItemCreateDTO);
                _response.Result = menuItemCreateDTO;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = [ex.ToString()];
                return BadRequest(_response);
            }

        }

        //更新菜品
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                var menuItem = await _menuItemService.UpdateMenuItemAsync(id, menuItemUpdateDTO);
                _response.Result = menuItemUpdateDTO;
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (KeyNotFoundException)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return BadRequest(_response);
            }
        }

        //删除菜品
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteMenuItemAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return NotFound(_response);
                }
                var isDelete = await _menuItemService.DeleteMenuItemAsync(id);
                if (!isDelete)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = [ex.Message];
                return BadRequest(_response);
            }
        }
    }
}
