using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopMercantec.Shared.DTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        // refresh token gere
        public DateTime AccessTokenExpiry {  get; set; }
        public string Message { get; set; } = String.Empty;
    }
}
