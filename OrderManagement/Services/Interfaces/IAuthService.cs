using Orders.DTOs.Request;
using Orders.DTOs.Response;

namespace Orders.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
