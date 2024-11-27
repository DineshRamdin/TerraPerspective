using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class GlobalParamDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Value { get; set; } = String.Empty;
        public string AdditionalValue { get; set; } = String.Empty;
        public string Comment { get; set; } = String.Empty;
        public bool AdminOnly { get; set; } = false;
        public bool Enable { get; set; } = false;
    }
}
