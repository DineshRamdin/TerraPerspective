using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_AccessRights : BaseAuditModel, IAuditable
    {
        [Key]
        public Guid id { get; set; }
        public bool hasRight { get; set; } = false;

        [ForeignKey("RoleId")]
        public virtual ApplicationRole Role { get; set; }

        [ForeignKey("ModuleId")]
        public virtual SYS_Modules Module { get; set; }
    }
}
