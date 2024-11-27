using BL.Constants;
using BL.Models.Common;
using BL.Services.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
	public class SendEmailService
	{
		public static BaseResponseDTO<bool> SendEmailAsync(string apiKey, string from, string to, string subject, string body,string contents,string pass,string url)
		{
			HttpClient client = new HttpClient();
			BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
			string emailResponse = "";
			//var requestUri = "https://api.smtpturbo.com/v1/send";
			var requestUri = url;
			var emailData = new
			{
				authuser = from,
				authpass = pass,
				from = from,
				to = to,
				subject = subject,
				content= contents,
				html_content = body
			};

			var json = Newtonsoft.Json.JsonConvert.SerializeObject(emailData);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

			var response =  client.PostAsync(requestUri, content).GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				dto.Data = true;
				dto.ErrorMessage = "Email sent successfully!";
				dto.QryResult = new QueryResult().SUCEEDED;

			}
			else
			{
				dto.Data = false;
				dto.ErrorMessage = $"Failed to send email. Status code: {response.StatusCode}";
				dto.QryResult = new QueryResult().FAILED;
			}

			return dto;
		}
	}
}
