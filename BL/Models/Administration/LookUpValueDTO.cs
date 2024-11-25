using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class LookUpValueDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string LookUpType { get; set; } = string.Empty;
    }

    public class LookUpValueCRUDDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public long LookUpType { get; set; }
    }
}
