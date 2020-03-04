using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinancialTrackerApi.Utils
{
    class Auth
    {
        public static TokenResponse GenerateJWToken(IConfiguration config, string userName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName)
            };

            var expiry = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Issuer"],
                claims,
                expires: expiry,
                signingCredentials: credentials);

            return new TokenResponse {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = expiry
            };
        }

        public static TokenResponse GenerateRefreshToken(IConfiguration config, string userName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:RefreshKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName)
            };

            var expiry = DateTime.Now.AddYears(1);

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Issuer"],
                claims,
                expires: expiry,
                signingCredentials: credentials);

            return new TokenResponse {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = expiry
            };
        }

        public static string GetUser(string tokenString)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
            if (token.ValidTo < DateTime.Now)
            {
                return null;
            }

            foreach (var claim in token.Claims)
            {
                if (claim.Type == JwtRegisteredClaimNames.Sub)
                {
                    return claim.Value;
                }
            }

            return null;
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetNonZeroBytes(salt = new byte[128]);
            return salt;
        }

        public static string GetUser(IEnumerable<Claim> claims)
        {
            string result = null;

            foreach (var claim in claims)
            {
                foreach (var property in claim.Properties)
                {
                    if (property.Value == JwtRegisteredClaimNames.Sub)
                    {
                        result = claim.Value;
                        break;
                    }
                }
            }

            return result;
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
