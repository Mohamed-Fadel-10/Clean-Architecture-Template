using System;

namespace Domain.Common.Models
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
