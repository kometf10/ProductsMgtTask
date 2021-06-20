using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.Requests
{
    public class RefreshTokenRequest
    {

        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }
}
