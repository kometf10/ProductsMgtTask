using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Requests;
using OA.Domain.Responces;
using OA.Domin.Requests;
using OA.Services.Auth;

namespace OA_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService identityService;

        public AuthController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
        {
            var authResult = await identityService.RegisterAsync(registerRequest);

            if (!authResult.Successed)
            {
                return Ok(authResult);
            }

            return Ok(authResult);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest registerRequest)
        {

            var authResult = await identityService.LoginAsync(registerRequest.Email, registerRequest.Password);

            if (!authResult.Successed)
            {
                return Ok(authResult);
            }
            return Ok(authResult);
        }

        [HttpPost("Refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {

            var authResult = identityService.RefreshToken(refreshTokenRequest);

            if (!authResult.Successed)
            {
                //if (authResult.Errors.Contains("Refresh Token Expired"))
                //    return Ok(authResult);

                return Ok(authResult);
            }

            return Ok(authResult);

        }


    }
}
