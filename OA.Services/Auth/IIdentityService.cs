using OA.Domain.Requests;
using OA.Domain.Responces;
using OA.Domin.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OA.Services.Auth
{
    public interface IIdentityService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest registerRequest);

        Task<AuthResult> LoginAsync(string Email, string Password);

        AuthResult RefreshToken(RefreshTokenRequest refreshToken);

    }
}
