using MangoFusionWithAI_APi.Data;
using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services.IServices;
using MangoFusionWithAI_APi.Utility;
using Microsoft.EntityFrameworkCore;

namespace MangoFusionWithAI_APi.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<OrderHeader>> GetOrdersAsync(string userId, bool isAdmin)
        {
            var query = _db.OrderHeaders
                .Include(u => u.OrderDetails)
                .ThenInclude(u => u.MenuItem)
                .OrderByDescending(u => u.OrderHeaderId)
                .AsQueryable();
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(u => u.ApplicationUserId == userId);
            }
            else if (!isAdmin)
            {
                return new List<OrderHeader>();
            }

            return await query.ToListAsync();
        }

        public async Task<OrderHeader?> GetOrderByIdAsync(int orderId)
        {
            return await _db.OrderHeaders
                 .Include(u => u.OrderDetails)
                 .ThenInclude(u => u.MenuItem)
                 .FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
        }

        public async Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO dto)
        {
            OrderHeader orderHeader = new OrderHeader()//创建订单头
            {
                PickUpName = dto.PickUpName,
                PickUpPhoneNumber = dto.PickUpPhoneNumber,
                PickUpEmail = dto.PickUpEmail,
                ApplicationUserId = dto.ApplicationUserId,
                TotalItem = dto.TotalItem,
                OrderTotal = dto.OrderTotal,
                OrderDate = DateTime.Now,
                Status = SD.status_confirmed
            };
            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();

            foreach (var detailDto in dto.OrderDetailsDTO)//创建订单详细
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId=orderHeader.OrderHeaderId,
                    MenuItemId = detailDto.MenuItemId,
                    Quantity = detailDto.Quantity,
                    ItemName = detailDto.ItemName,
                    Price = detailDto.Price,

                };
                _db.OrderDetails.Add(orderDetail);
            }
            await _db.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<(bool Succeeded, string Error)> UpdateOrderAsync(int orderId, OrderHeaderUpdateDTO dto)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
            if (orderFromDb == null)
            {
                return (false, "订单不存在");
            }
            if (!string.IsNullOrEmpty(dto.PickUpName))
            {
                orderFromDb.PickUpName = dto.PickUpName;
            }
            if (!string.IsNullOrEmpty(dto.PickUpPhoneNumber))
            {
                orderFromDb.PickUpPhoneNumber = dto.PickUpPhoneNumber;
            }
            if (!string.IsNullOrEmpty(dto.PickUpEmail))
            {
                orderFromDb.PickUpEmail = dto.PickUpEmail;
            }

            if (!string.IsNullOrEmpty(orderFromDb.Status))
            {
                //订单状态流转
                if (orderFromDb.Status.Equals(SD.status_confirmed, StringComparison.InvariantCultureIgnoreCase)
                    && dto.Status.Equals(SD.status_readyForPickUp, StringComparison.InvariantCultureIgnoreCase)) 
                {
                    orderFromDb.Status = SD.status_readyForPickUp;
                }
                else if (orderFromDb.Status.Equals(SD.status_readyForPickUp, StringComparison.InvariantCultureIgnoreCase)
                    && dto.Status.Equals(SD.status_Completed, StringComparison.InvariantCultureIgnoreCase))
                {
                    orderFromDb.Status = SD.status_Completed;
                }
                else if (dto.Status.Equals(SD.status_Cancelled, StringComparison.InvariantCultureIgnoreCase))
                {
                    orderFromDb.Status = SD.status_Cancelled;
                }

            }
            await _db.SaveChangesAsync();
            return (true, "");
        }

        public async Task<(bool Succeeded, string Error)> UpdateRatingAsync(int orderDetailsId, int rating)
        {
            var orderFromDb = await _db.OrderDetails.FirstOrDefaultAsync(u => u.OrderDetailId == orderDetailsId);
            if (orderFromDb is null)
            {
                return (false, "订单不存在");
            }
            orderFromDb.Rating = rating;
            await _db.SaveChangesAsync();
            return (true, "");
        }
    }
}
