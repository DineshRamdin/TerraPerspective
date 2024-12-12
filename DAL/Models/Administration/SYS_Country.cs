using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_Country : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;

    }
}
