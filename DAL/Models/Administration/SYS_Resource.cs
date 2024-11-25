using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_Resource : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Surname { get; set; }
        public string Othername { get; set; }
        public string Type { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }

        [ForeignKey("UserId")]
        public virtual SYS_User User { get; set; }
    }
}
