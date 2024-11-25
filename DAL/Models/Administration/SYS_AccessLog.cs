using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_AccessLog : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("RoomId")]
        public virtual SYS_Room Room { get; set; }

        [ForeignKey("ResourceId")]
        public virtual SYS_Resource Resource { get; set; }

        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }


        [ForeignKey("PurposeId")]
        public virtual SYS_LookUpValue? Purpose { get; set; }
        //public string Purpose { get; set; }
        public string Remarks { get; set; }
        public string Visitor { get; set; }
    }
}
