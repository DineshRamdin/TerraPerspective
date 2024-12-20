using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Context;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.IO;
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
				List<DropdownNode> dropdownNodelst = new List<DropdownNode>();
				foreach (MatrixListDTO m in MLL)
				{
     //               DropdownNode dropdownNode = new DropdownNode(); 
					//dropdownNode.Id = m.GMID;
     //               dropdownNode.Text = m.GMDescription;
     //               dropdownNodelst.Add(dropdownNode);
					ML.AddRange(GetMatrixChild(m.GMID));
					//dropdownNodelst = GetMatrixChildDropDown(m.GMID, dropdownNodelst);
				}
				dt.ParentIDS = MLL.Select(y => y.GMID).ToList();
				dt.UID = UsrId;
				dt.AUID = AID;
				dt.IDS = ML.Select(y => y.GMID).ToList();
                //dt.dropdownNodelst = dropdownNodelst;
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

		//public static List<DropdownNode> GetMatrixChildDropDown(long GMID,List<DropdownNode> dropdownNode)
		//{
		//	List<MatrixListDTO> ml = new List<MatrixListDTO>();
		//	List<MatrixListDTO> MLL = new List<MatrixListDTO>();
  //          List<ChildNode> childNodes = new List<ChildNode>();
		//	PerspectiveContext context = new PerspectiveContext();
		//	try
		//	{
		//		ml = (from a in context.SYS_GroupMatrix
		//			  where a.ParentGMID == GMID && a.DeleteStatus == false
		//			  select new MatrixListDTO()
		//			  {
		//				  GMID = a.GMID,
		//				  GMDescription = a.GMDescription
		//			  }).ToList();
		//		if (ml.Count > 0)
		//		{
		//			MLL.AddRange(ml);
  //                  foreach (var item in dropdownNode)
  //                  {
  //                      if (item.Id == GMID)
  //                      {
		//					foreach (MatrixListDTO m in ml)
		//					{
		//						ChildNode child = new ChildNode();
		//						child.Id = m.GMID;
		//						child.Text = m.GMDescription;
		//						childNodes.Add(child);
		//						MLL.AddRange(GetMatrixChild(m.GMID));
		//					}
		//					item.Children = childNodes;
		//				}
  //                  }
					
		//		}

		//	}
		//	catch (Exception ex)
		//	{

		//	}
		//	return dropdownNode;
		//}

		public BaseResponseDTO<List<CRUDMatrix>> GetTree()
        {
            BaseResponseDTO<List<CRUDMatrix>> BaseDto = new BaseResponseDTO<List<CRUDMatrix>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();

                mlRoot = (from a in context.SYS_GroupMatrix
                          where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                          select new CRUDMatrix()
                          {
                              id = a.GMID.ToString(),
                              text = a.GMDescription,
                              parent = "#",
                          }).ToList();
                ml = (from a in context.SYS_GroupMatrix
                      where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                      select new CRUDMatrix()
                      {
                          id = a.GMID.ToString(),
                          text = a.GMDescription,
                          parent = a.ParentGMID.ToString(),
                      }).ToList();
                ml.AddRange(mlRoot);
                
				BaseDto.Data = ml;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = ml;
                BaseDto.ErrorMessage = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<List<OutputNode>> GetTreeDropdown()
		{
			BaseResponseDTO<List<OutputNode>> BaseDto = new BaseResponseDTO<List<OutputNode>>();
			List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
			List<CRUDMatrix> ml = new List<CRUDMatrix>();
			List<OutputNode> outputNodes = new List<OutputNode>();
			PerspectiveContext context = new PerspectiveContext();
			try
			{
				UserMatrixDTO UM = sGetMatrixUserByUserID();

				mlRoot = (from a in context.SYS_GroupMatrix
						  where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
						  select new CRUDMatrix()
						  {
							  id = a.GMID.ToString(),
							  text = a.GMDescription,
							  parent = "#",
						  }).ToList();
				ml = (from a in context.SYS_GroupMatrix
					  where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
					  select new CRUDMatrix()
					  {
						  id = a.GMID.ToString(),
						  text = a.GMDescription,
						  parent = a.ParentGMID.ToString(),
					  }).ToList();
				ml.AddRange(mlRoot);
				var nodeMap = ml.ToDictionary(node => node.id, node => new OutputNode
				{
					Title = node.text,
					Checked = node.state.Checked,
					Href = $"#{node.id}",
					DataAttrs = new List<DataAttr>
					{
						new DataAttr { Title = "value", Data = node.id }
					}
				});

				foreach (var node in ml)
				{
					if (node.parent != "#")
					{
						nodeMap[node.parent].Data.Add(nodeMap[node.id]);
					}
				}

				outputNodes = nodeMap.Values
					.Where(node => ml.Any(n => n.parent == "#" && n.id == node.Href.TrimStart('#')))
					.ToList();

				BaseDto.Data = outputNodes;
				BaseDto.QryResult = new QueryResult().SUCEEDED;
			}
			catch (Exception ex)
			{
				BaseDto.Data = outputNodes;
				BaseDto.ErrorMessage = "An Error Occured";
				BaseDto.QryResult = new QueryResult().FAILED;
			}
			return BaseDto;
		}

		public UserMatrixDTO sGetMatrixUserByUserIDParam(string Id)
        {
            UserMatrixDTO dt = new UserMatrixDTO();
            PerspectiveContext context = new PerspectiveContext();
            try
            {

                string AID = context.Users.Where(x => x.Id.ToLower() == Id).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.SYS_User.Where(x => x.AId == AID).Select(x=>x.Id).FirstOrDefault();
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

        public BaseResponseDTO<List<CRUDMatrix>> GetTreeUser(string UserId)
        {
            BaseResponseDTO<List<CRUDMatrix>> BaseDto = new BaseResponseDTO<List<CRUDMatrix>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();
                UserMatrixDTO UMId = sGetMatrixUserByUserIDParam(UserId);

                mlRoot = (from a in context.SYS_GroupMatrix
                          where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                          select new CRUDMatrix()
                          {
                              id = a.GMID.ToString(),
                              text = a.GMDescription,
                              parent = "#",
                              state = new state()
                              {
                                  Checked = false,
                                  Opened = true
                              }
                          }).ToList();
                ml = (from a in context.SYS_GroupMatrix
                      where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                      select new CRUDMatrix()
                      {
                          id = a.GMID.ToString(),
                          text = a.GMDescription,
                          parent = a.ParentGMID.ToString(),
                      }).OrderBy(x=>x.id).ToList();
                ml.AddRange(mlRoot);
                foreach (long gmid in UMId.ParentIDS)
                {
                    ml.Where(x => x.id == gmid.ToString()).Select(w => { w.state.Checked = true; return w; }).ToList();
                }
				
                BaseDto.Data = ml;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = ml;
                BaseDto.ErrorMessage = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

		public BaseResponseDTO<List<OutputNode>> GetTreeDropdownUser(string UserId)
		{
			BaseResponseDTO<List<OutputNode>> BaseDto = new BaseResponseDTO<List<OutputNode>>();
			List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
			List<CRUDMatrix> ml = new List<CRUDMatrix>();
			List<OutputNode> outputNodes = new List<OutputNode>();
			PerspectiveContext context = new PerspectiveContext();
			try
			{
				UserMatrixDTO UM = sGetMatrixUserByUserID();
				UserMatrixDTO UMId = sGetMatrixUserByUserIDParam(UserId);

				mlRoot = (from a in context.SYS_GroupMatrix
						  where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
						  select new CRUDMatrix()
						  {
							  id = a.GMID.ToString(),
							  text = a.GMDescription,
							  parent = "#",
							  state = new state()
							  {
								  Checked = false,
								  Opened = true
							  }
						  }).ToList();
				ml = (from a in context.SYS_GroupMatrix
					  where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
					  select new CRUDMatrix()
					  {
						  id = a.GMID.ToString(),
						  text = a.GMDescription,
						  parent = a.ParentGMID.ToString(),
					  }).OrderBy(x => x.id).ToList();
				ml.AddRange(mlRoot);
				foreach (long gmid in UMId.ParentIDS)
				{
					ml.Where(x => x.id == gmid.ToString()).Select(w => { w.state.Checked = true; return w; }).ToList();
				}
				List<CRUDMatrix> mlf = new List<CRUDMatrix>();
				mlf = ml.Where(x => x.state.Checked == true).ToList();

				var nodeMap = mlf.ToDictionary(node => node.id, node => new OutputNode
				{
					Title = node.text,
					Checked = node.state.Checked,
					Href = $"#{node.id}",
					DataAttrs = new List<DataAttr>
					{
						new DataAttr { Title = "value", Data = node.id }
					}
				});

				foreach (var node in mlf)
				{
					if (node.parent != "#")
					{
						if (node.parent != "#" && nodeMap.ContainsKey(node.parent))
						{
							nodeMap[node.parent].Data.Add(nodeMap[node.id]);
						}
					}
				}

				if (mlf.Any(n => n.parent.Contains("#")))
				{
					// Filter the outputNodes to include only nodes with valid parents or no parents
					outputNodes = nodeMap.Values
					.Where(node => mlf.Any(n => n.parent == "#" && n.id == node.Href.TrimStart('#')))
					.ToList();
				}
				else
				{
					// Filter the outputNodes to include only nodes with valid parents or no parents
					outputNodes = nodeMap.Values
						.Where(node => mlf.All(n => n.parent != "#"))
						.ToList();
				}
				BaseDto.Data = outputNodes;
				BaseDto.QryResult = new QueryResult().SUCEEDED;
			}
			catch (Exception ex)
			{
				BaseDto.Data = outputNodes;
				BaseDto.ErrorMessage = "An Error Occured";
				BaseDto.QryResult = new QueryResult().FAILED;
			}
			return BaseDto;
		}

		public BaseResponseDTO<bool> CRUDM(List<CRUDMatrix> dt)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();
                bool Incoming =  HandleIncoming(dt);
                bool Update = HandleUpdate(dt);
                bool Removed = HandleRemoved(UM, dt);
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
                if (!Incoming && !Removed)
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "An Error Occured";
                    BaseDto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.ErrorMessage = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

        public bool HandleIncoming(List<CRUDMatrix> dt)
        {
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> Incoming = new List<CRUDMatrix>();
            bool status = false;
            try
            {
                Incoming = dt.Where(x => x.id.Contains("j1_")).ToList();
                mlRoot = Incoming.Where(x => !x.parent.Contains("j1_")).ToList();
                foreach (var matrix in mlRoot)
                {
                    long result = CreateMatrix(Convert.ToInt64(matrix.parent), matrix.text);
                    if (result > 0)
                    {
                        Incoming.Where(x => x.parent == matrix.id).Select(c => { c.parent = result.ToString(); return c; }).ToList();
                        Incoming.Where(x => x.id == matrix.id).Select(c => { c.id = result.ToString(); return c; }).ToList();
                    }
                }
                if (Incoming.Where(x => x.id.Contains("j1_")).ToList().Count > 0)
                {
                    status = HandleIncoming(Incoming);
                    if (!status)
                    {
                        return status;
                    }
                }
                status = true;
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public bool HandleUpdate(List<CRUDMatrix> dt)
        {
            List<CRUDMatrix> update = new List<CRUDMatrix>();
            PerspectiveContext context = new PerspectiveContext();
            bool status = false;
            try
            {
                update = dt.Where(x => !x.id.Contains("j1_")).ToList();
                foreach (CRUDMatrix u in update)
                {
                    SYS_GroupMatrix gm = context.SYS_GroupMatrix.Where(x => x.GMID == Convert.ToInt64(u.id)).FirstOrDefault();
                    if (gm.GMDescription != u.text)
                    {
                        gm.GMDescription = u.text;
                        context.SYS_GroupMatrix.Update(gm);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public bool HandleRemoved(UserMatrixDTO UM, List<CRUDMatrix> dt)
        {
            List<long> Removed = new List<long>();
            bool status = false;
            try
            {
                List<long> Existing = GetExisting(dt);
                Removed = UM.IDS;
                Removed.RemoveAll(x => Existing.Contains(x));
                status = true;
                status = DeleteMatrix(Removed);
            }
            catch (Exception ex)
            {

            }
            return status;
        }

        public List<long> GetExisting(List<CRUDMatrix> dt)
        {
            List<long> Existing = new List<long>();
            try
            {
                foreach (string e in dt.Where(x => !x.id.Contains("j1_")).Select(y => y.id).ToList())
                {
                    Existing.Add(Convert.ToInt64(e));
                }
            }
            catch (Exception ex)
            {

            }
            return Existing;
        }
        public long CreateMatrix(long pgmid, string gmdes)
        {
            PerspectiveContext context = new PerspectiveContext();
            long id = 0;
            try
            {
                SYS_GroupMatrix gm = new SYS_GroupMatrix()
                {
                    ParentGMID = pgmid,
                    GMDescription = gmdes,
                    Remarks = string.Empty,
                    IsCompany = true,
                    CompanyCode = string.Empty,
                    LegalName = string.Empty,
                };
                context.SYS_GroupMatrix.Add(gm);
                context.SaveChanges();
                id = gm.GMID;
            }
            catch (Exception ex)
            {

            }
            return id;
        }
        public bool DeleteMatrix(List<long> Removed)
        {
            PerspectiveContext context = new PerspectiveContext();
            bool status = false;
            try
            {
                foreach (long Id in Removed)
                {
                    SYS_GroupMatrix gm = context.SYS_GroupMatrix.Where(x => x.GMID == Id).FirstOrDefault();
                    gm.DeleteStatus = true;
                    context.SYS_GroupMatrix.Update(gm);
                    context.SaveChanges();
                }
                status = true;
            }
            catch (Exception ex)
            {

            }
            return status;
        }

        public BaseResponseDTO<bool> SaveUserM(List<long> Ids, string AUID)
        {
            PerspectiveContext context = new PerspectiveContext();
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            BaseResponseDTO.Data = false;
            try
            {
                long SysUId = context.SYS_User.Where(x => x.AId == AUID).FirstOrDefault().Id;
                foreach (long Id in Ids)
                {
                    SYS_GroupMatrixUser gm = new SYS_GroupMatrixUser()
                    {
                        IID = SysUId,
                        GMID = Id
                    };
                    context.SYS_GroupMatrixUser.Add(gm);
                    context.SaveChanges();
                }
                BaseResponseDTO.Data = true;
                BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
				BaseResponseDTO.ErrorMessage = "User save Successfully";
			}
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
                BaseResponseDTO.QryResult = new QueryResult().FAILED;
            }
            return BaseResponseDTO;
        }
        public BaseResponseDTO<bool> UpdateUserM(List<long> Ids, string AUID)
        {
            PerspectiveContext context = new PerspectiveContext();
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            BaseResponseDTO.Data = false;
            try
            {
                long SysUId = context.SYS_User.Where(x => x.AId == AUID).FirstOrDefault().Id;
                List<long> GmUL = context.SYS_GroupMatrixUser.Where(x => x.IID == SysUId && x.DeleteStatus == false).Select(y => y.GMID).ToList();
                foreach (long Id in Ids)
                {
                    if (!GmUL.Contains(Id))
                    {
                        SYS_GroupMatrixUser gm = new SYS_GroupMatrixUser()
                        {
                            IID = SysUId,
                            GMID = Id
                        };
                        context.SYS_GroupMatrixUser.Add(gm);
                        context.SaveChanges();

                    }
                    else
                    {
                        GmUL.RemoveAll(x => x == Id);
                    }

                }
                foreach (long Id in GmUL)
                {
                    SYS_GroupMatrixUser gm = context.SYS_GroupMatrixUser.Where(x => x.GMID == Id && x.IID == SysUId && x.DeleteStatus == false).FirstOrDefault();
                    if (gm != null)
                    {
                        gm.DeleteStatus = true;
                        context.SYS_GroupMatrixUser.Update(gm);
                        context.SaveChanges();
                    }

                }
                BaseResponseDTO.Data = true;
                BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
				BaseResponseDTO.ErrorMessage = "User Update Successfully";
			}
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
                BaseResponseDTO.QryResult = new QueryResult().FAILED;
            }
            return BaseResponseDTO;
        }

        #region Zone Metrix

        public BaseResponseDTO<List<CRUDMatrix>> GetTreeForZone(long Id)
        {
            BaseResponseDTO<List<CRUDMatrix>> BaseDto = new BaseResponseDTO<List<CRUDMatrix>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                ZoneMatrixDTO UM = sGetMatrixZoneByZoneID(Id);

                mlRoot = (from a in context.SYS_GroupMatrix
                          where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                          select new CRUDMatrix()
                          {
                              id = a.GMID.ToString(),
                              text = a.GMDescription,
                              parent = "#",
                          }).ToList();
                ml = (from a in context.SYS_GroupMatrix
                      where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                      select new CRUDMatrix()
                      {
                          id = a.GMID.ToString(),
                          text = a.GMDescription,
                          parent = a.ParentGMID.ToString(),
                      }).ToList();
                ml.AddRange(mlRoot);
                BaseDto.Data = ml;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = ml;
                BaseDto.ErrorMessage = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

        public static ZoneMatrixDTO sGetMatrixZoneByZoneID(long Id)
        {
            ZoneMatrixDTO dt = new ZoneMatrixDTO();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                //IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                //string claimsPrincipal = null;
                //if (httpContextAccessor.HttpContext != null)
                //{
                //    claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                //    if (claimsPrincipal == null)
                //    {
                //        claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
                //    }
                //}
                //else
                //{
                //    claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
                //}
                //string AID = context.Users.Where(x => x.Email.ToLower() == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
                //long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                //ml = (from a in context.NPF_SYS_GroupMatrixUser
                //      join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                //      where a.IID == UsrId select a.GMID).ToList();
                List<MatrixListDTO> MLL = (from a in context.SYS_ZoneMatrix
                                           join b in context.SYS_GroupMatrix on a.GMID equals b.GMID
                                           where a.IID == Id && a.DeleteStatus == false && b.DeleteStatus == false
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
                dt.ZID = Id;
                dt.ZAUID = new GeoJsonWriter().Write(context.SYS_ZoneManagement.Where(x => x.Id == Id).Select(x => x.GeomColumn).FirstOrDefault());

                dt.IDS = ML.Select(y => y.GMID).ToList();
                

            }
            catch (Exception ex)
            {

            }
            return dt;
        }

		public BaseResponseDTO<bool> SaveZoneM(List<long> Ids, long ZoneId)
		{
			PerspectiveContext context = new PerspectiveContext();
			BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
			BaseResponseDTO.Data = false;
			try
			{
				//long SysUId = context.SYS_User.Where(x => x.AId == Id).FirstOrDefault().Id;
				foreach (long Id in Ids)
				{
					SYS_ZoneMatrix gm = new SYS_ZoneMatrix()
					{
						IID = ZoneId,
						GMID = Id
					};
					context.SYS_ZoneMatrix.Add(gm);
					context.SaveChanges();
				}
				BaseResponseDTO.Data = true;
				BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
				BaseResponseDTO.ErrorMessage = "Zone Management Data save Successfully";
			}
			catch (Exception ex)
			{
				BaseResponseDTO.Data = false;
				BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
				BaseResponseDTO.QryResult = new QueryResult().FAILED;
			}
			return BaseResponseDTO;
		}
		public BaseResponseDTO<bool> UpdateZoneM(List<long> Ids, long ZoneId)
		{
			PerspectiveContext context = new PerspectiveContext();
			BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
			BaseResponseDTO.Data = false;
			try
			{
				//long SysUId = context.SYS_User.Where(x => x.AId == AUID).FirstOrDefault().Id;
				List<long> GmUL = context.SYS_ZoneMatrix.Where(x => x.IID == ZoneId && x.DeleteStatus == false).Select(y => y.GMID).ToList();
				foreach (long Id in Ids)
				{
					if (!GmUL.Contains(Id))
					{
						SYS_ZoneMatrix gm = new SYS_ZoneMatrix()
						{
							IID = ZoneId,
							GMID = Id
						};
						context.SYS_ZoneMatrix.Add(gm);
						context.SaveChanges();

					}
					else
					{
						GmUL.RemoveAll(x => x == Id);
					}

				}
				foreach (long Id in GmUL)
				{
					SYS_ZoneMatrix gm = context.SYS_ZoneMatrix.Where(x => x.GMID == Id && x.IID == Id && x.DeleteStatus == false).FirstOrDefault();
					if (gm != null)
					{
						gm.DeleteStatus = true;
						context.SYS_ZoneMatrix.Update(gm);
						context.SaveChanges();
					}

				}
				BaseResponseDTO.Data = true;
				BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
				BaseResponseDTO.ErrorMessage = "Zone Management Data update Successfully";
			}
			catch (Exception ex)
			{
				BaseResponseDTO.Data = false;
				BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
				BaseResponseDTO.QryResult = new QueryResult().FAILED;
			}
			return BaseResponseDTO;
		}

		public BaseResponseDTO<bool> SaveProjectM(List<long> Ids, long ProjectId)
		{
			PerspectiveContext context = new PerspectiveContext();
			BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
			BaseResponseDTO.Data = false;
			try
			{
				//long SysUId = context.SYS_User.Where(x => x.AId == Id).FirstOrDefault().Id;
				foreach (long Id in Ids)
				{
					SYS_ProjectsMatrix gm = new SYS_ProjectsMatrix()
					{
						IID = ProjectId,
						GMID = Id
					};
					context.SYS_ProjectsMatrix.Add(gm);
					context.SaveChanges();
				}
				BaseResponseDTO.Data = true;
				BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
				BaseResponseDTO.ErrorMessage = "Project Data save Successfully";
			}
			catch (Exception ex)
			{
				BaseResponseDTO.Data = false;
				BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
				BaseResponseDTO.QryResult = new QueryResult().FAILED;
			}
			return BaseResponseDTO;
		}
		public BaseResponseDTO<bool> UpdateProjectM(List<long> Ids, long ProjectId)
		{
			PerspectiveContext context = new PerspectiveContext();
			BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
			BaseResponseDTO.Data = false;
			try
			{
				//long SysUId = context.SYS_User.Where(x => x.AId == AUID).FirstOrDefault().Id;
				List<long> GmUL = context.SYS_ProjectsMatrix.Where(x => x.IID == ProjectId && x.DeleteStatus == false).Select(y => y.GMID).ToList();
				foreach (long Id in Ids)
				{
					if (!GmUL.Contains(Id))
					{
						SYS_ProjectsMatrix gm = new SYS_ProjectsMatrix()
						{
							IID = ProjectId,
							GMID = Id
						};
						context.SYS_ProjectsMatrix.Add(gm);
						context.SaveChanges();

					}
					else
					{
						GmUL.RemoveAll(x => x == Id);
					}

				}
				foreach (long Id in GmUL)
				{
					SYS_ProjectsMatrix gm = context.SYS_ProjectsMatrix.Where(x => x.GMID == Id && x.IID == Id && x.DeleteStatus == false).FirstOrDefault();
					if (gm != null)
					{
						gm.DeleteStatus = true;
						context.SYS_ProjectsMatrix.Update(gm);
						context.SaveChanges();
					}

				}
				BaseResponseDTO.Data = true;
				BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
				BaseResponseDTO.ErrorMessage = "Project Data update Successfully";
			}
			catch (Exception ex)
			{
				BaseResponseDTO.Data = false;
				BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
				BaseResponseDTO.QryResult = new QueryResult().FAILED;
			}
			return BaseResponseDTO;
		}

		#endregion
	}
}
