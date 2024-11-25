using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
   public class LookUpTypeDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
