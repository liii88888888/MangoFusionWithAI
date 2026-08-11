using MangoFusionWithAI_APi.Data;
using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace MangoFusionWithAI_APi.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public MenuItemService(ApplicationDbContext db,IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<List<MenuItem>> GetAllMenuItemAsync()
        {
            var menuItems = await _db.MenuItems.ToListAsync();
            var orderDetailsWithRatings = await _db.OrderDetails
                .Where(u => u.Rating != null)
                .ToListAsync();

            foreach (var menuItem in menuItems)//菜品赋值评分
            {
                var ratings= orderDetailsWithRatings
                    .Where(u => u.MenuItemId == menuItem.Id)
                    .Select(u => u.Rating!.Value);
                menuItem.Rating=ratings.Any() ? ratings.Average() : 0;
            }

            return menuItems;

        }

        public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);

            if (menuItem == null) return null;

            var orderDetailsWithRatings = await _db.OrderDetails
                .Where(u => u.Rating != null && u.MenuItemId == menuItem.Id)
                .ToListAsync();

            var ratings = orderDetailsWithRatings.Select(u => u.Rating!.Value);
            menuItem.Rating = ratings.Any() ? ratings.Average() : 0;

            return menuItem;
        }

        public async Task<MenuItem> CreateMenuItemAsync(MenuItemCreateDTO dto)
        {
            var imagesPath = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            var filePath = Path.Combine(imagesPath, dto.File.FileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var menuItem = new MenuItem
            {
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                SpecialTag = dto.SpecialTag,
                Price = dto.Price,
                Image = "images/" + dto.File.FileName,
            };

            _db.MenuItems.Add(menuItem);
            await  _db.SaveChangesAsync();
            return menuItem;
        }

        public async Task<MenuItem> UpdateMenuItemAsync(int id,MenuItemUpdateDTO dto)
        {
            var menuItemFromDb = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id)
                    ?? throw new KeyNotFoundException($"Id : {id} 未找到");

            menuItemFromDb.Name = dto.Name;
            menuItemFromDb.Category = dto.Category;
            menuItemFromDb.Description = dto.Description;
            menuItemFromDb.SpecialTag = dto.SpecialTag;
            menuItemFromDb.Price = dto.Price;

            //更新图片（可选）
            if (dto.File != null && dto.File.Length > 0)
            {
                var imagesPath = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                // 删除旧图片
                var oldFilePath = Path.Combine(_env.WebRootPath, menuItemFromDb.Image);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }

                // 保存新图片
                var newFilePath = Path.Combine(imagesPath, dto.File.FileName);
                if (File.Exists(newFilePath))
                {
                    File.Delete(newFilePath);
                }

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                menuItemFromDb.Image = "images/" + dto.File.FileName;
            }

            _db.MenuItems.Update(menuItemFromDb);
            await _db.SaveChangesAsync();
            return menuItemFromDb;
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var menuItemFromDb = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);
            if(menuItemFromDb is null) throw new KeyNotFoundException($"Id : {id} 未找到");

            var oldFilePath = Path.Combine(_env.WebRootPath, menuItemFromDb.Image);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }

            _db.MenuItems.Remove(menuItemFromDb);
            await  _db.SaveChangesAsync();

            return true;
        }

        
    }
}
