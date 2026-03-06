using Domain.Entities;
using Domain.Entities.RefreshToken.RefreshToken;
using Domain.ResultPattern;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyProject.Infrastructure.Identity;
using Persistence.ExternalServices.JWTService.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.JWTService
{
    public class JWTService : IJWTService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _settings;

        public JWTService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> options)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<TokenResponse> GenerateTokenAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var (token, expiration) = await GenerateJWToken(user);
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpirationDate = expiration
            };
        }

        private async Task<List<Claim>> GetClaimsForUser(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
            var basicClaims = CreateBasicClaims(user);
            basicClaims.AddRange(userClaims);
            basicClaims.AddRange(roleClaims);
            return basicClaims;
        }

        private List<Claim> CreateBasicClaims(ApplicationUser user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("uid", user.Id.ToString()),
                new Claim("domain_uid", user.DomainUserId.ToString())
            };
        }

        private async Task<(JwtSecurityToken, DateTime)> GenerateJWToken(ApplicationUser user)
        {
            var claims = await GetClaimsForUser(user);
            var tokenExpiration = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);
            var signingCredentials = GetSigningCredentials();
            var jwt = CreateJwtSecurityToken(claims, tokenExpiration, signingCredentials);
            return (jwt, tokenExpiration);
        }

        private SigningCredentials GetSigningCredentials()
        {
            if (string.IsNullOrWhiteSpace(_settings.Key))
                throw new InvalidOperationException("JWT Secret is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        private JwtSecurityToken CreateJwtSecurityToken(
            IEnumerable<Claim> claims,
            DateTime expires,
            SigningCredentials signingCredentials)
        {
            return new JwtSecurityToken(
                issuer: string.IsNullOrWhiteSpace(_settings.Issuer) ? null : _settings.Issuer,
                audience: string.IsNullOrWhiteSpace(_settings.Audience) ? null : _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow, 
                expires: expires,
                signingCredentials: signingCredentials
            );
        }

        public (string PlainToken, RefreshToken TokenEntity) CreateRefreshToken()
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Fill(randomBytes);
            var plainToken = Convert.ToBase64String(randomBytes);
            var tokenHash = ComputeSha256Hash(plainToken);

            var refreshToken = new RefreshToken
            {
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                RevokedAt = null,
                IsRevoked = false
            };

            return (plainToken, refreshToken);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_settings.Key);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        public long GetTokenExpirationTime(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwt;
            try
            {
                jwt = handler.ReadJwtToken(token);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Token is not a valid JWT");
            }

            var expClaim = jwt.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (!string.IsNullOrEmpty(expClaim) && long.TryParse(expClaim, out var expNumeric))
                return expNumeric;

            return new DateTimeOffset(jwt.ValidTo).ToUnixTimeSeconds();
        }

        public string ComputeSha256Hash(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hashBytes);
        }
    }
}