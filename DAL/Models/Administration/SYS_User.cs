using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_User : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string AId { get; set; }//AspUsers Id
        public UserType Type { get; set; } = UserType.User;
        public bool IsLoginAllowed { get; set; } = true;
    }
}
