using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
	public static class APIErrorMessage
	{
		public static string TokenExpire = "Token expired. Please log in again.";
		public static string RequestmodelBlank = "Invalid request.";
		public static string Invalidmodel = "Invalid data.";
		public static string CommonError = "Something is wrong. please contact to administration";
		public static string InvalidCredentials = "Invalid Credentials!!";
		public static string LoginFailed = "Login Failed. You don't have access on this system";
	}
}
