using DAL.Common;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class SaveARDTO
    {
        public SYS_AccessRights AccessRights { get; set; } = new SYS_AccessRights();
        public List<SYS_AccessRightsDetails> ARD { get; set; } = new List<SYS_AccessRightsDetails>();

    }
    public class AccessRightsDTO
    {
        public string AccessRightId { get; set; }
        public string RoleName { get; set; }

        //roleId
        public string Id { get; set; }
        public string GMenuId { get; set; }
        public string GMenu { get; set; }
        public string MenuId { get; set; }
        public string Menu { get; set; }
        public string SubMenuId { get; set; }
        public string SubMenu { get; set; }
        public bool hasRight { get; set; }
        public string email { get; set; }
        public string UserId { get; set; }
        public int order { get; set; }
        public int cntModules { get; set; }
    }

	public class MainARDTO
	{
		public string RoleName { get; set; }
		public string RoleId { get; set; }
		public List<AccessRightDTO> AccessRightDTO { get; set; } = new List<AccessRightDTO>();
	}

	public class AccessRightDTO
	{
		public Guid Id { get; set; }//ModuleIds
		public string Level1 { get; set; } = string.Empty;
		public string Level2 { get; set; } = string.Empty;
		public string Level3 { get; set; } = string.Empty;
		public string Level4 { get; set; } = string.Empty;
		public List<AccessRightDetailDTO> AccessRightDetailDTO { get; set; } = new List<AccessRightDetailDTO>();
	}

	public class AccessRightDetailDTO
	{
		public long? Id { get; set; }
		public string OperationTypeName { get; set; }
		public AccessOperationType OperationType { get; set; }
		public bool Permission { get; set; } = false;

	}
}
