using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _IConfiguration;

        private QueryResult queryResult;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _IConfiguration = configuration;
            queryResult = new QueryResult();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            BaseResponseDTO<ApplicationUser> Result = new BaseResponseDTO<ApplicationUser>();

            if (!ModelState.IsValid)
            {
                Result.ErrorMessage = APIErrorMessage.Invalidmodel;
                Result.QryResult = queryResult.FAILED;
                return BadRequest(Result);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            Result = await LoginService.Login(model, _signInManager, _userManager, _roleManager, Convert.ToString(ipAddress), "");
            if (Result.QryResult == queryResult.SUCEEDED)
            {
                Result.Token = GenerateJwtToken(Result.Data, Result.Role, Result.RoleId.ToString());
                return Ok(Result);
            }
            else
            {
                Result.Data = null;
                return BadRequest(Result);
            }
        }

        private string GenerateJwtToken(ApplicationUser model, string Role, string RoleId)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_IConfiguration["JwtSettings:Secret"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);            

            var claims = new List<Claim>() {
            new Claim("id",model.Id.ToString()),
            new Claim("Email",model.Email.ToString()),
            new Claim("Role",Role.ToString()),
            new Claim("RoleId",RoleId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _IConfiguration["JwtSettings:Issuer"],
                audience: _IConfiguration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }   
}
