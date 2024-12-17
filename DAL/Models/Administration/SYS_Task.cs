using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_Task : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string UserCode { get; set; }
        public string Taskname { get; set; }
        public string TaskDescription { get; set; }
        [ForeignKey("Project")]
        public virtual SYS_Projects Projects { get; set; }
		[ForeignKey("ParentTask")]
		public virtual SYS_Task? Task { get; set; }
		public long Status { get; set; }
        public int? Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsVisible { get; set; }
    }
}
