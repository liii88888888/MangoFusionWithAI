using MangoFusionWithAI_APi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace MangoFusionWithAI_APi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    Id = 1,
                    Name = "春卷",
                    Description = "外皮金黄酥脆，内馅蔬菜鲜香，是非常受欢迎的餐前小食。",
                    Image = "images/spring_roll.jpg",
                    Price = 7.99,
                    Category = "开胃小菜",
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 2,
                    Name = "印度炸三角饺",
                    Description = "外壳炸至酥脆，内部填充土豆与香料，风味浓郁独特。",
                    Image = "images/samosa.jpg",
                    Price = 8.99,
                    Category = "开胃小菜",
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 3,
                    Name = "鲜醇例汤",
                    Description = "慢火熬制的浓汤，口感温润，饭前饮用开胃暖身。",
                    Image = "images/soup.jpg",
                    Price = 8.99,
                    Category = "开胃小菜",
                    SpecialTag = "畅销爆款"
                },
                new MenuItem
                {
                    Id = 4,
                    Name = "特色炒面",
                    Description = "面条筋道弹牙，搭配蔬菜与秘制酱汁，口味丰富。",
                    Image = "images/noodles.jpg",
                    Price = 10.99,
                    Category = "主菜",
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 5,
                    Name = "蔬菜咖喱配面包",
                    Description = "绵密的混合蔬菜咖喱，搭配松软餐包，异域风味十足。",
                    Image = "images/pav_bhaji.jpg",
                    Price = 12.99,
                    Category = "主菜",
                    SpecialTag = "高分推荐"
                },
                new MenuItem
                {
                    Id = 6,
                    Name = "芝士奶酪披萨",
                    Description = "饼底烘烤香脆，铺满香浓奶酪，配料丰富，奶香浓郁。",
                    Image = "images/pizza.jpg",
                    Price = 11.99,
                    Category = "主菜",
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 7,
                    Name = "芒果盛宴",
                    Description = "选用新鲜芒果制作，果香饱满，清甜爽口，饭后解腻佳品。",
                    Image = "images/mango_paradise.jpg",
                    Price = 13.99,
                    Category = "甜品",
                    SpecialTag = "厨师特选"
                },
                new MenuItem
                {
                    Id = 8,
                    Name = "胡萝卜甜心",
                    Description = "胡萝卜制成的特色甜点，口感绵密，甜而不腻。",
                    Image = "images/carrot_love.jpg",
                    Price = 4.99,
                    Category = "甜品",
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 9,
                    Name = "香甜奶卷",
                    Description = "软糯香甜的奶味甜卷，入口绵柔，适合饭后享用。",
                    Image = "images/sweet_rolls.jpg",
                    Price = 4.99,
                    Category = "甜品",
                    SpecialTag = "厨师特选"
                });
        }
    }
}
