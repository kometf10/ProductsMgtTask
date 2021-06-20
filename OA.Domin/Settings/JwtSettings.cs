using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA.Domain.Settings
{
    public class JwtSettings
    {
        public string Secret { set; get; }

        public string ExpiresInHours { get; set; }

        public string RefreshExpiresInDays { get; set; }

    }
}
