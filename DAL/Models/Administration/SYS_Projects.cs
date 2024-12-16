using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_Projects : BaseAuditModel, IAuditable
    {
		[Key]
		public long Id { get; set; }
		public string UserCode { get; set; }
		public string ProjectName { get; set; }
		public string? ProjectDetails { get; set; }
		public string ProjectDescription { get; set; }

		[ForeignKey("AssignTo")]
		public virtual SYS_User User { get; set; }

		[ForeignKey("ProjectTemplateId")]
		public virtual SYS_ProjectTemplate? ProjectTemplate { get; set; }
		public string? ProjectColorCode { get; set; }

		public TimeOnly PlannedHours { get; set; }

		public long Status { get; set; }
		public string StatusDetails { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool? IsVisible { get; set; }
	}
}
