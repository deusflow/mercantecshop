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
        public string AccessTokenExpiry {  get; set; }
        public string Message { get; set; }
    }
}
