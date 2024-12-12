using BL.Models.Common;
using DAL.Context;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BL.Services.Common
{
	public class GenerateCode
	{

		public async Task<BaseResponseDTO<string>> GCode()
		{
			BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
			PerspectiveContext context = new PerspectiveContext();
			try
			{
				SYS_GlobalParam Gp = context.SYS_GlobalParam.Where(x => x.Name == "UniqueCodeGenerator").FirstOrDefault();
				Gp.Value = (Convert.ToInt32(Gp.Value) + 1).ToString();
				context.SYS_GlobalParam.Update(Gp);
				context.SaveChanges();
				int length = Gp.Value.Length;
				baseResponseDTO.Data = Gp.Value.Length < 6 ? Gp.Value.PadLeft(6, '0') : Gp.Value;
			}
			catch (Exception ex)
			{
				int _min = 100000000;
				int _max = 999999999;
				Random _rdm = new Random();
				baseResponseDTO.Data = _rdm.Next(_min, _max).ToString();
			}

			return baseResponseDTO;

		}

		public BaseResponseDTO<string> GCodeMain(string UserToken, string TableName)
		{
			BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
			PerspectiveContext context = new PerspectiveContext();
			DateTime now = DateTime.Now;
			Guid Uid = Guid.Parse(UserToken);
			int compid = 0;
			string code = "";
			string pref = "";
			string value = "";
			string format = "";
			try
			{
				SYS_Company cl = context.SYS_Company.Where(x => x.NameofCompany == "Default").FirstOrDefault();
				compid =Convert.ToInt32(cl.Id);
				var usr = context.Users.Where(x => x.UserToken == Uid).FirstOrDefault();
				if (usr != null)
				{
					if (usr.Company != 0)
					{
						compid = usr.Company;
					}
				}
				var tblconfig = (from a in context.SYS_TableCodeConfigurations
								 join b in context.SYS_CodeConfiguration on a.ConfigurationId equals b.Id
								 where a.CompanyId == compid && a.TableName == TableName
								 select new
								 {
									 a.Prefix,
									 b.Date,
									 b.DateFormat,
									 b.Month,
									 b.MonthFormat,
									 b.Year,
									 b.YearFormat,
									 b.UsePrefix,
									 b.PaddingNo,
									 IsReset = b.Reset == null ? false : Convert.ToBoolean(b.Reset),
									 b.ResetConfig
								 }
								 ).FirstOrDefault();
				if (tblconfig != null)
				{

					if (tblconfig.UsePrefix == true)
					{
						pref = tblconfig.Prefix;
					}
					if (tblconfig.Year == true)
					{
						format = tblconfig.YearFormat;
						if (tblconfig.Month == true)
						{
							format += tblconfig.MonthFormat;
							if (tblconfig.Date == true)
							{
								format += tblconfig.DateFormat;
							}
						}
						try
						{
							format = now.ToString(format);
						}
						catch (Exception ex)
						{

						}
						code += format;
					}


					if (tblconfig.ResetConfig == "Year" && tblconfig.Year == true)
					{
						value = GetCode(code, tblconfig.IsReset, tblconfig.ResetConfig, TableName);
					}
					else if (tblconfig.ResetConfig == "Month" && tblconfig.Year == true && tblconfig.Month == true)
					{
						value = GetCode(code, tblconfig.IsReset, tblconfig.ResetConfig, TableName);
					}
					else if (tblconfig.ResetConfig == "Day" && tblconfig.Year == true && tblconfig.Month == true && tblconfig.Date == true)
					{
						value = GetCode(code, tblconfig.IsReset, tblconfig.ResetConfig, TableName);
					}
					else
					{
						value = GetCode(code, false, tblconfig.ResetConfig, TableName);
					}

					int length = value.Length + code.Length;
					if (length < tblconfig.PaddingNo)
					{
						value = value.PadLeft(tblconfig.PaddingNo, '0');
					}

					code += value;

				}
				else
				{
					code = ( new GenerateCode().GCode()).ToString();
				}

			}
			catch (Exception ex)
			{
				code = (new GenerateCode().GCode()).ToString();
			}
			baseResponseDTO.Data = pref + code;
			return baseResponseDTO;

		}

		public static string GetCode(string Code, bool Isreset, string reset, string Table)
		{
			string baseResponseDTO = "";
			PerspectiveContext context = new PerspectiveContext();
			DateTime now = DateTime.Now;
			try
			{
				switch (Table)
				{
					case "SYS_Projects":
						{
							var dt = context.SYS_Projects.OrderByDescending(y => y.Id).FirstOrDefault();
							if (dt != null)
							{
								if (Isreset)
								{
									return Generator(Code, dt.UserCode, dt.CreatedDate, reset);
								}
								else
								{
									string Num = (GetNumber(dt.UserCode)).Remove(0, Code.Length);
									return (Convert.ToInt32(Num) + 1).ToString();
								}
							}
							return "1";
						}
					case "SYS_Task":
						{
							var dt = context.SYS_Task.OrderByDescending(y => y.Id).FirstOrDefault();
							if (dt != null)
							{
								if (Isreset)
								{
									return Generator(Code, dt.UserCode, dt.CreatedDate, reset);
								}
								else
								{
									string Num = (GetNumber(dt.UserCode)).Remove(0, Code.Length);
									return (Convert.ToInt32(Num) + 1).ToString();
								}
							}
							return "1";
						}

				}

			}
			catch (Exception ex)
			{
				int _min = 100000000;
				int _max = 999999999;
				Random _rdm = new Random();
				baseResponseDTO = "0121" + _rdm.Next(_min, _max).ToString();
			}

			return baseResponseDTO;

		}

		public static string Generator(string Code, string dtcode, DateTime Createddate, string reset)
		{
			DateTime now = DateTime.Now;
			if (reset == "Year")
			{
				if (Createddate.Year == now.Year)
				{
					return GenNumber(dtcode, Code);
				}
			}
			else if (reset == "Month")
			{
				if (Createddate.Year == now.Year && Createddate.Month == now.Month)
				{
					return GenNumber(dtcode, Code);
				}
			}
			else if (reset == "Day")
			{
				if (Createddate.Year == now.Year && Createddate.Month == now.Month && Createddate.Day == now.Day)
				{
					return GenNumber(dtcode, Code);
				}

			}
			return "1";
		}

		public static string GetNumber(string input)
		{
			return Regex.Replace(input, "[^0-9.]", "");
		}

		public static string GenNumber(string dtcode, string Code)
		{
			string Num = (GetNumber(dtcode)).Remove(0, Code.Length);
			return (Convert.ToInt32(Num) + 1).ToString();
		}
	}
}
