using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OA_API.Extentions
{
    public static class IdentityExtention
    {

        public static string GetUserId(this ClaimsPrincipal User)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            return (userIdClaim != null) ? userIdClaim.Value : "";

        }

        public static string GetUserPositionClaim(this ClaimsPrincipal user)
        {
            var userClaims = user.Claims.Where(c => c.Type == "Permission" && c.Value.Contains("position"));

            return (userClaims.Count() == 1) ? userClaims.FirstOrDefault().Value : "";
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            var adminClaim = user.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            return adminClaim.Any();

        }


    }
}
