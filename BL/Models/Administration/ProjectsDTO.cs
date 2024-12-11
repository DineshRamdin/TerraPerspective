using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ProjectsDTO
    {
		public long Id { get; set; }
		public string UserCode { get; set; }
		public string ProjectName { get; set; }
		public string ProjectDetails { get; set; }
		public string ProjectDescription { get; set; }
		public string ProjectManager { get; set; }
		public long Status { get; set; }
		public string StatusDetails { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string PlannedHours { get; set; }
		public string IsVisible { get; set; }
	}

	public class ProjectsCRUDDTO
	{
		public long Id { get; set; }
		public string UserCode { get; set; }
		public string ProjectName { get; set; }
		public string ProjectDetails { get; set; } = string.Empty;
		public string ProjectDescription { get; set; }
		public long AssignTo { get; set; }
		public long Status { get; set; }
		public string StatusDetails { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public TimeOnly PlannedHours { get; set; }
		public Nullable<bool> IsVisible { get; set; }
	}
}
