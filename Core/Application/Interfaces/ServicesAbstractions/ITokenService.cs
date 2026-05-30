using Core.Domain.Entities.Auth;

namespace Core.Application.Interfaces.ServicesAbstractions
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(AppUser user);
    }
}
