using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_Device : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }

        public DeviceType Type { get; set; }

        [ForeignKey("DefaultCarouselId")]
        public virtual SYS_Carousel DefaultCarousel { get; set; }
    }
}
