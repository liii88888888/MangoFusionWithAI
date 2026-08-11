using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;

namespace MangoFusionWithAI_APi.Services.IServices
{
    public interface IMenuItemService
    {
        Task<List<MenuItem>> GetAllMenuItemAsync();//查所有菜品
        Task<MenuItem?> GetMenuItemByIdAsync(int id);//查指定菜品
        Task<MenuItem> CreateMenuItemAsync(MenuItemCreateDTO menuItemCreateDTO);//新增菜品
        Task<MenuItem> UpdateMenuItemAsync(int id,MenuItemUpdateDTO menuItemUpdateDTO);//更新菜品
        Task<bool> DeleteMenuItemAsync(int id);//删除菜品
    }
}
