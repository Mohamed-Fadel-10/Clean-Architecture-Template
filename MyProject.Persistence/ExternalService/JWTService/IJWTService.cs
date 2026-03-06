using Domain.Entities;
using Domain.Entities.RefreshToken;
using Domain.Entities.RefreshToken.RefreshToken;
using MyProject.Infrastructure.Identity;
using Persistence.ExternalServices.JWTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.JWTService
{
    public interface IJWTService
    {
        Task<TokenResponse> GenerateTokenAsync(ApplicationUser user);
        ClaimsPrincipal? ValidateToken(string token);
        long GetTokenExpirationTime(string token);
        (string PlainToken, RefreshToken TokenEntity) CreateRefreshToken();
        string ComputeSha256Hash(string input);
    }
}
