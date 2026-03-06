
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.JWTService.Models
{
    public class NewRefreshTokenResponse
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Roles { get; set; }

    }
}
