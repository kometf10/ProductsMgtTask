using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.DataAccess
{
    public class User : IdentityUser
    {
        public string RefreshTokn { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public bool FirstUse { get; set; } = true;

        public bool Online { get; set; }

        public DateTime? LastPasswordChange { get; set; }

        public string ProfilePic { get; set; }

        public bool ConfirmedAccount { get; set; }


    }
}
