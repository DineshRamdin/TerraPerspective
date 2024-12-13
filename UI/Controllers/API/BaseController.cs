using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace UI.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class BaseController : ControllerBase
	{
		public string GetClaimsFromToken(string Key)
		{
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadJwtToken(token); // Decode the token

			var Value = jwtToken?.Claims.FirstOrDefault(c => c.Type == Key)?.Value;

			return Value;
		}    
    }
}
