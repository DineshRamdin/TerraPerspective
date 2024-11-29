using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class SystemIconDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } = String.Empty;        
        public string? SystemIconImagebase64 { get; set; }
        public bool? IsImage { get; set; }

    }
}
