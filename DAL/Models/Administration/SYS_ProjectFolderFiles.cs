using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
	public class SYS_ProjectFolderFiles : BaseAuditModel, IAuditable
	{
		public long Id { get; set; }
		public string? Folder { get; set; }
		public long ProjectId { get; set; }
	}
}
