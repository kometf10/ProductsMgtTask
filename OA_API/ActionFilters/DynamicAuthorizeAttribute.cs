using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA_API.ActionFilters
{
    public class DynamicAuthorizeAttribute : AuthorizeAttribute
    {

        public DynamicAuthorizeAttribute(string policy) : base(policy)
        {
            
        } 

    }
}
