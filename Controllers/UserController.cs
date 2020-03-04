using FinancialTrackerApi.Models;
using FinancialTrackerApi.Services;
using FinancialTrackerApi.Utils;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace FinancialTrackerApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger _logger;
        private IConfiguration _config;

        public UserController(IConfiguration config, UserService userService, ILogger<UserController> logger)
        {
            _config = config;
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(AuthForm login)
        {
            _logger.LogInformation("Login initiated");
            IActionResult response = Unauthorized();

            var user = _userService.Get(login.UserName);

            if (user == null)
            {
                _logger.LogWarning("User {User} not found", login.UserName);
                return response;
            }

            var argon2Config = new Argon2Config
            {
                Threads = Environment.ProcessorCount,
                Password = Encoding.UTF8.GetBytes(login.Password),
                Salt = user.Salt,
                HashLength = 128
            };
            if (Argon2.Verify(user.Password, argon2Config))
            {
                _logger.LogInformation("User {User} logged in", login.UserName);
                var accessToken = Auth.GenerateJWToken(_config, user.UserName);
                var refreshToken = Auth.GenerateRefreshToken(_config, user.UserName);

                user.AccessToken = accessToken.Token;
                user.RefreshToken = refreshToken.Token;
                user.AccessExpiry = accessToken.Expiry;

                _userService.Update(user.Id, user);

                return Ok(new
                {
                    accessToken = accessToken.Token,
                    refreshToken = refreshToken.Token
                });
            }

            _logger.LogWarning("User {User} provided wrong password", login.UserName);
            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(AuthForm register)
        {
            _logger.LogInformation("Registration initiated");
            var user = _userService.Get(register.UserName);

            if (user != null)
            {
                _logger.LogWarning("Duplicate user {User} found", register.UserName);
                return Conflict(new { message = @"Username already exists" });
            }

            var salt = Auth.GenerateSalt();

            var argon2Config = new Argon2Config
            {
                Threads = Environment.ProcessorCount,
                Password = Encoding.UTF8.GetBytes(register.Password),
                Salt = salt,
                HashLength = 128
            };
            var hashedPassword = Argon2.Hash(argon2Config);

            var accessToken = Auth.GenerateJWToken(_config, register.UserName);
            var refreshToken = Auth.GenerateRefreshToken(_config, register.UserName);

            user = new User
            {
                UserName = register.UserName,
                Password = hashedPassword,
                Salt = salt,
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                AccessExpiry = accessToken.Expiry
            };

            _ = _userService.Add(user);

            return Ok(new 
            { 
                accessToken = accessToken.Token,
                refreshToken = refreshToken.Token
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Renew([FromBody]string token)
        {
            var userName = Auth.GetUser(token);
            if (userName == null)
            {
                return Unauthorized();
            }

            var user = _userService.Get(userName);

            if (user == null ||
                user.RefreshToken != token ||
                user.AccessExpiry.ToLocalTime() > DateTime.Now)
            {
                return Unauthorized();
            }

            var accessToken = Auth.GenerateJWToken(_config, userName);

            user.AccessToken = accessToken.Token;
            user.AccessExpiry = accessToken.Expiry;
            _userService.Update(user.Id, user);

            return Ok(new
            {
                accessToken = accessToken.Token
            });
        }
    }
}