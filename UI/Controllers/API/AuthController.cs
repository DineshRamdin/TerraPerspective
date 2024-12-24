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
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace UI.Controllers.API
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

			Result = await LoginService.Login(model, _signInManager, _userManager, _roleManager,Convert.ToString(ipAddress), "");

			if (Result.QryResult == queryResult.SUCEEDED)
			{
				Result.Token = GetJWTToken(Result.Data,Result.Role,Result.RoleId.ToString());
				return Ok(Result);
			}
			else
			{
				Result.Data = null;
				return BadRequest(Result);
			}			
		}

		private string GetJWTToken(ApplicationUser model,string Role,string RoleId)
		{
			var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_IConfiguration["JWTConfiguration:Secret"]));
			var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>() {
			new Claim("id",model.Id.ToString()),
			new Claim("Email",model.Email.ToString()),
			new Claim("Role",Role.ToString()),
			new Claim("RoleId",RoleId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var tokenOptions = new JwtSecurityToken(
				issuer: _IConfiguration["JWTConfiguration:Issuer"],
				audience: _IConfiguration["JWTConfiguration:Audience"],
				expires: DateTime.Now.AddHours(3),
				claims: claims,
				signingCredentials: signingCredentials
				);
			
			return new JwtSecurityTokenHandler().WriteToken(tokenOptions); ;
		}
	}
}
