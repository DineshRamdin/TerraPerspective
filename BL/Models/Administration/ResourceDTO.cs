using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ResourceDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string AId { get; set; }
        public string Surname { get; set; }
        public string Othername { get; set; }
        public string Type { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
    }
}
