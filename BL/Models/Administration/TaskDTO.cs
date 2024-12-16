using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class TaskDTO
    {
        public long Id { get; set; }
        public string UserCode { get; set; }
        public string Taskname { get; set; }
        public string TaskDescription { get; set; }
        public string ProjectName { get; set; }
        public string ParentTaskName { get; set; }
        public string Status { get; set; }
        public string StatusDetails { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string IsVisible { get; set; }
    }

    public class TaskCRUDDTO
    {
        public long Id { get; set; }
        public string UserCode { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public long Project { get; set; }
        public long? ParentTask { get; set; }
        public long Status { get; set; }
        public string StatusDetails { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Nullable<bool> IsVisible { get; set; }
    }
}
