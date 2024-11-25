using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_CarouselPosterMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        public long CarouselId { get; set; }

        [ForeignKey("PosterId")]
        public virtual SYS_Poster Poster { get; set; }



    }
}
