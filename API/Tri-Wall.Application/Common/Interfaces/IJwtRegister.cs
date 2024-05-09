
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Common.Interfaces;

public interface IJwtRegister
{
    Task<JwtResponse> GenerateToken(string account);
    Task<JwtResponse> GenerateRefreshToken();
}
