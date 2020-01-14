using FinancialTrackerApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinancialTrackerApi.Utils
{
    class Auth
    {
        public static string GenerateJWToken(IConfiguration config, User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
            };

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateSalt()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetNonZeroBytes(salt = new byte[16]);
            return Encoding.UTF8.GetString(salt);
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
}
