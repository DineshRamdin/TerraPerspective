using BL.Models.Administration;
using DAL.Context;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BL.Models.Administration.MatrixDTO;

namespace BL.Services.Common
{
	public class MatrixService
	{
		public static UserMatrixDTO sGetMatrixUserByUserID()
		{
			UserMatrixDTO dt = new UserMatrixDTO();
			PerspectiveContext context = new PerspectiveContext();
			try
			{
				IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
				string claimsPrincipal = null;
				if (httpContextAccessor.HttpContext != null)
				{
					claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
					if (claimsPrincipal == null)
					{
						claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
					}
				}
				else
				{
					claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
				}
				string AID = context.Users.Where(x => x.Email.ToLower() == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
				long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
				//ml = (from a in context.NPF_SYS_GroupMatrixUser
				//      join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
				//      where a.IID == UsrId select a.GMID).ToList();
				List<MatrixListDTO> MLL = (from a in context.SYS_GroupMatrixUser
										   join b in context.SYS_GroupMatrix on a.GMID equals b.GMID
										   where a.IID == UsrId && a.DeleteStatus == false && b.DeleteStatus == false
										   select new MatrixListDTO()
										   {
											   GMID = a.GMID,
											   GMDescription = b.GMDescription
										   }).ToList();
				List<MatrixListDTO> ML = new List<MatrixListDTO>();
				ML.AddRange(MLL);
				foreach (MatrixListDTO m in MLL)
				{
					ML.AddRange(GetMatrixChild(m.GMID));
				}
				dt.ParentIDS = MLL.Select(y => y.GMID).ToList();
				dt.UID = UsrId;
				dt.AUID = AID;
				dt.IDS = ML.Select(y => y.GMID).ToList();

			}
			catch (Exception ex)
			{

			}
			return dt;
		}

		public static List<MatrixListDTO> GetMatrixChild(long GMID)
		{
			List<MatrixListDTO> ml = new List<MatrixListDTO>();
			List<MatrixListDTO> MLL = new List<MatrixListDTO>();
			PerspectiveContext context = new PerspectiveContext();
			try
			{
				ml = (from a in context.SYS_GroupMatrix
					  where a.ParentGMID == GMID && a.DeleteStatus == false
					  select new MatrixListDTO()
					  {
						  GMID = a.GMID,
						  GMDescription = a.GMDescription
					  }).ToList();
				if (ml.Count > 0)
				{
					MLL.AddRange(ml);
					foreach (MatrixListDTO m in ml)
					{
						MLL.AddRange(GetMatrixChild(m.GMID));
					}
				}

			}
			catch (Exception ex)
			{

			}
			return MLL;
		}

	}
}
