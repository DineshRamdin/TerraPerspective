using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_ZoneMatrix : BaseAuditModel, IAuditable
    {
        [Key]
        public long MID { get; set; }
        public long IID { get; set; } //Zone Id
        public long GMID { get; set; }//MATRIX Id
    }
}
