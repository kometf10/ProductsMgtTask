using Microsoft.AspNetCore.Identity;
using OA.Domain.Responces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using OA.Domain.Settings;
using OA.DataAccess;
using System.Security.Cryptography;
using OA.Domin.Requests;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Requests;

namespace OA.Services.Auth
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<User> userManager;
        private readonly JwtSettings jwtSettings;
        private readonly AppDbContext dbContext;

        public IdentityService(UserManager<User> userManager, 
                               JwtSettings jwtSettings, 
                               AppDbContext dbContext)
        {
            this.userManager = userManager;
            this.jwtSettings = jwtSettings;
            this.dbContext = dbContext;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest registerRequest)
        {
            var email = registerRequest.Email;
            var password = registerRequest.Password;

            AuthResult authResult = new AuthResult();

            var existedUser = await userManager.FindByEmailAsync(email);
            if (existedUser != null)
            {
                return new AuthResult {Successed = false, Errors = new List<string>{ "This Email Address Already Existed" } };
            }

            var newUser = new User { Email = email, UserName = email, ConfirmedAccount = true };

            var createdUser = await userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthResult() { Successed = false, Errors = createdUser.Errors.Select(x => x.Description) };
            }
            
            var token = GenerateUserJwtToken(newUser);
            var refreshToken = GenerateRefreshToken();

            //Set Refresh Token To User
            newUser.RefreshTokn = refreshToken;
            newUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToDouble(jwtSettings.RefreshExpiresInDays));
            await userManager.UpdateAsync(newUser);

            if (token != "")
            {
                authResult.Successed = true;
                authResult.Token = token;
                authResult.RefreshToken = refreshToken;
                return authResult;
            }
            return new AuthResult() { Successed = false, Errors = new List<string>() { "Faild To Register User" } };
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var valid = await userManager.CheckPasswordAsync(user, password);
                if (valid)
                {
                    AuthResult authResult = new AuthResult();

                    var token = GenerateUserJwtToken(user);
                    var refreshToken = GenerateRefreshToken();

                    //Set Refresh Token To User
                    user.RefreshTokn = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToDouble(jwtSettings.RefreshExpiresInDays));
                    await userManager.UpdateAsync(user);

                    if (token != "")
                    {
                        authResult.Successed = true;
                        authResult.Token = token;
                        authResult.RefreshToken = refreshToken;
                        authResult.ProfilePic = user.ProfilePic;
                        return authResult;
                    }
                    return new AuthResult() { Successed = false, Errors = new List<string>() { "Faild To Login" } };
                }
            }

            return new AuthResult { Errors = new string[] { "Invalid Login Data" } };
        }

        public AuthResult RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest == null)
                return new AuthResult { Successed = false, Errors = new List<string>() { "Invalid Client Request" } };

            string userId = GetUserIdFromExpiredToken(refreshTokenRequest.Token);
            var user = dbContext.Users.Find(userId);

            if (user == null ||
               user.RefreshTokn != refreshTokenRequest.RefreshToken)
            {
                return new AuthResult { Successed = false, Errors = new List<string>() { "Invalid Client Request" } };
            }
            if(user.RefreshTokenExpiryTime < DateTime.Now)
            {
                return new AuthResult { Successed = false, Errors = new List<string>() { "Refresh Token Expired" } };
            }

            var token = GenerateUserJwtToken(user);
            var refreshtoken = GenerateRefreshToken();

            user.RefreshTokn = refreshtoken;
            dbContext.Entry(user).State = EntityState.Modified;
            dbContext.SaveChanges();

            return new AuthResult()
            {
                Successed = true,
                Token = token,
                RefreshToken = user.RefreshTokn
            };

        }

        private string GenerateUserJwtToken(User user)
        {
            try
            {
                var signingCredentials = GetSigningCredentials();
                var claims = GetClaims(user.Id);
                var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return token;
            }
            catch (Exception)
            {
                //Log The Exception
                return "";
            }  
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(string userId)
        {
            var user = dbContext.Users.Find(userId);

            //Basic Claims
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserName", user.UserName),
                new Claim("FirstUse", user.FirstUse.ToString()),
                new Claim("LastPasswordChange", user.LastPasswordChange.ToString())
            };

            //Role Claims
            var userRoles = dbContext.UserRoles.Where(ur => ur.UserId == userId).ToList();
            foreach (var userRole in userRoles) {
                var role = dbContext.Roles.Find(userRole.RoleId);
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

                //Role Premission Claims
                var roleClaims = dbContext.RoleClaims.Where(rc => rc.RoleId == role.Id).ToList();
                foreach(var roleClaim in roleClaims)
                    claims.Add(new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
                
            }


            return claims;

        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                //issuer: jwtSettings.issuar,
                //audience: jwtSettings.validAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(jwtSettings.ExpiresInHours)),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var rng = RandomNumberGenerator.Create();
            
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
            
        }

        private string GetUserIdFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateLifetime = false,
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                string userId = principal.Claims.First(x => x.Type == "Id").Value;

                return userId;
            }
            catch(Exception e)
            {
                var msg = e.Message;
                return "";
            }

        }

        private bool IsAdmin(User user)
        {
            var userRoles = dbContext.UserRoles.Where(ur => ur.UserId == user.Id).ToList();

            foreach (var userRole in userRoles)
            {
                var role = dbContext.Roles.Find(userRole.RoleId);
                if (role.Name == "Admin")
                    return true;
            }

            return false;
           
        }

    }
}
