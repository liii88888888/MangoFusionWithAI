using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusionWithAI_APi.Models
{
    public class OrderHeader
    {
        [Key]//买家属性
        public int OrderHeaderId { get; set; } 
        [Required]
        public string PickUpName { get; set; } = string.Empty;
        [Required]
        public string PickUpPhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PickUpEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        //订单
        public double OrderTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItem { get; set; }

        //链接订单详细
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}

