using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Server.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Server.DTO.JWTAuthentication;
using Server.Entities;
using Server.DTO.Shared;
using Server.Constants;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> userManager;

        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration
        )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await userManager.FindByNameAsync(model.Username!);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password!))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!)
                );

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                        authSigningKey,
                        SecurityAlgorithms.HmacSha256
                    )
                );

                return Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    }
                );
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO input)
        {
            var userExists = await userManager.FindByNameAsync(input.Username!);
            if (userExists != null)
                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "User already exists!" }
                );

            var user = new User()
            {
                Email = input.Email!,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = input.Username,
                Name = input.Name,
                Surname = input.Surname,
                PhoneNumber = input.PhoneNumber,
                Address = input.Address,
                BirthDate = input.BirthDate,
                Gender = input.Gender,
            };
            var result = await userManager.CreateAsync(user, input.Password!);
            if (!result.Succeeded)
                return StatusCode(
                    500,
                    new InternalServerErrorDTO
                    {
                        Message = "User creation failed! Please check user details and try again."
                    }
                );

            // Assign role "User" to the user
            if (!await roleManager.RoleExistsAsync(RoleNames.User))
                await roleManager.CreateAsync(new IdentityRole<int>(RoleNames.User));

            if (await roleManager.RoleExistsAsync(RoleNames.User))
            {
                await userManager.AddToRoleAsync(user, RoleNames.User);
            }

            return Ok("User created successfully!");
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username!);
            if (userExists != null)
                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "User already exists!" }
                );

            var user = new User
            {
                Email = model.Email!,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
                return StatusCode(
                    500,
                    new InternalServerErrorDTO
                    {
                        Message = "User creation failed! Please check user details and try again."
                    }
                );

            //if (!await roleManager.RoleExistsAsync(RoleNames.Administrator))
            //    await roleManager.CreateAsync(new IdentityRole(RoleNames.Administrator));
            //if (!await roleManager.RoleExistsAsync(RoleNames.User))
            //    await roleManager.CreateAsync(new IdentityRole(RoleNames.User));

            //if (await roleManager.RoleExistsAsync(RoleNames.Administrator))
            //{
            //    await userManager.AddToRoleAsync(user, RoleNames.Administrator);
            //}

            return Ok("User created successfully!");
        }
    }
}

