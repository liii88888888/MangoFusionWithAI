using MangoFusionWithAI_APi.Dto;
using MangoFusionWithAI_APi.Models;

namespace MangoFusionWithAI_APi.Services.IServices
{
    public interface IAuthService
    {
        Task<(bool Successed, List<string> Errors)> RegisterAsync(RegisterRequestDTO modle);
        Task<(bool Successed, LoginResponseDTO? Result ,List<string> Errors)> LoginAsync(LoginRequestDTO modle);
    }
}
