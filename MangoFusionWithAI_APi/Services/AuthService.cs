using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;
using MangoFusionWithAI_APi.Services.IServices;
using MangoFusionWithAI_APi.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MangoFusionWithAI_APi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _secretKey;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret") ?? "";
        }

        public async Task<(bool Successed, List<string> Errors)> RegisterAsync(RegisterRequestDTO model)
        {
            var errors= new List<string>();
            ApplicationUser newUser = new()
            {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name,
                NormalizedEmail = model.Email.ToUpper()
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                errors.AddRange(result.Errors.Select(e => e.Description));
                return (false, errors);
            }



            //首次注册自动创建角色
            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
            }

            //声明管理员或顾客角色
            if (model.Role.Equals(SD.Role_Admin, StringComparison.CurrentCultureIgnoreCase))
            {
                await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
            }
            else
            {
                await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
            }

            return (true, errors);

        }

        public async Task<(bool Successed, LoginResponseDTO? Result, List<string> Errors)> LoginAsync(LoginRequestDTO model)
        {
            var errors = new List<string>();
            var userFromDb = await _userManager.FindByEmailAsync(model.Email);

            if (userFromDb==null)
            {
                errors.Add("账号或密码不正确");
                return (false, null, errors);
            }

            var isValid =await _userManager.CheckPasswordAsync(userFromDb, model.Password);

            if (!isValid)
            {
                errors.Add("账号或密码不正确");
                return (false, null, errors);
            }

            //生成JWT令牌
            var role = (await _userManager.GetRolesAsync(userFromDb)).FirstOrDefault() ?? "";
            var token = GenerateJwtToken(userFromDb, role);

            var loginResponse = new LoginResponseDTO
            {
                Email = model.Email,
                Role = role,
                Token=token
            };

            return (true, loginResponse, errors);
        }

        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            JwtSecurityTokenHandler tokenHandler = new ();
            byte[] key = Encoding.ASCII.GetBytes(_secretKey);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("fullname", user.Name),
                    new Claim("id", user.Id),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
