using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domain.Responces
{
    public class AuthResult
    {
        public IEnumerable<string> Errors { get; set; }

        public bool Successed { get; set; }        

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string ProfilePic { get; set; }

        //public CustomUser User { get; set; }
    }
}
