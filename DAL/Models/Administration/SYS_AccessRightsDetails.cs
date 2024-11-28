using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Common;

namespace DAL.Models.Administration
{
    public class SYS_AccessRightsDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("AccessRightId")]
        public virtual SYS_AccessRights AccessRights { get; set; }
        public AccessOperationType AOT { get; set; }
        public bool Permission { get; set; } = false;
    }
}
