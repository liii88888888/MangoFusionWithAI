using System.ComponentModel.DataAnnotations;

namespace MangoFusionWithAI_APi.Dto
{
    public class OrderHeaderCreateDTO
    {
        
        [Required]
        public string PickUpName { get; set; } = string.Empty;
        [Required]
        public string PickUpPhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PickUpEmail { get; set; } = string.Empty;
        //店家属性
        public string ApplicationUserId { get; set; } = string.Empty;
        //订单
        public double OrderTotal { get; set; }
        public int TotalItem { get; set; }
        public List<OrderDetailsCreateDTO> OrderDetailsDTO { get; set; } = new();


    }
}