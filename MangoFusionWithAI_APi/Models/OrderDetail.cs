using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusionWithAI_APi.Models
{
    public class OrderDetail
    {
        [Key]//Id关联
        public int OrderDetailId { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [Required]
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public MenuItem? MenuItem { get; set; }

        //实际订单详细 (关于具体菜品的详细信息）
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string ItemName { get; set; } = string.Empty;
        public int? Rating { get; set; } = null;
        [Required]
        public double Price { get; set; } 
    }
}

