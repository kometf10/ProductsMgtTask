using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OA_API.Extentions
{
    public static class PolicyAuthorizationExtention
    {

        public static IServiceCollection AddPolicyBasedAuthorization(this IServiceCollection services)
        {

            services.AddAuthorizationCore(options => {

                options.AddPolicy("products-managment", policy => policy.RequireAssertion(context => AdminOrCan(context, "products-managment")));
                options.AddPolicy("catigories-managment", policy => policy.RequireAssertion(context => AdminOrCan(context, "catigories-managment")));

            });

            return services;
        }

        public static bool AdminOrCan(AuthorizationHandlerContext context, string claim)
        {
            bool isAdmin = context.User.IsInRole("Admin");
            bool can = context.User.HasClaim("Permission", claim);

            return isAdmin || can;
        }
        public static bool AdminOrCan(AuthorizationHandlerContext context, List<string> claims)
        {
            bool isAdmin = context.User.IsInRole("Admin");
           
            foreach (string c in claims)
                if (context.User.HasClaim("Permission", c))
                    return true;

            return isAdmin;

        }

        public static bool Can(AuthorizationHandlerContext context, string claim)
        {
            return context.User.HasClaim("Permission", claim);
        }

        public static bool Can(AuthorizationHandlerContext context, List<string> claims)
        {
            foreach (string c in claims)
                if (context.User.HasClaim("Permission", c))
                    return true;            

            return false;
        }

    }
}
