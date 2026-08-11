using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;

namespace MangoFusionWithAI_APi.Services.IServices
{
    public interface IOrderService
    {
        Task<List<OrderHeader>> GetOrdersAsync(string userId, bool isAdmin);
        Task<OrderHeader?> GetOrderByIdAsync(int orderId);
        Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO dto);
        Task<(bool Succeeded, string Error)> UpdateOrderAsync(int orderId, OrderHeaderUpdateDTO dto);
        Task<(bool Succeeded, string Error)> UpdateRatingAsync(int orderDetailsId, int rating);
    }
}
