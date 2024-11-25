using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class PosterDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? PosterImageBase64 { get; set; }
        public long Type { get; set; }
        public string TypeName { get; set; }
        public bool Status { get; set; }
    }
}
