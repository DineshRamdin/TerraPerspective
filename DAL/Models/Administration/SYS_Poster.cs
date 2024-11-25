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
    public class SYS_Poster : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string? PosterImageBase64 { get; set; }
        public PosterType Type { get; set; }
        public bool Status { get; set; }
    }
}
