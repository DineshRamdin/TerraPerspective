using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_ProjectTemplateMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long ProjectTemplateID { get; set; }
        public string? TaskName { get; set; }
        public int Duration { get; set; }
        public int Sequence { get; set; }

    }
}
