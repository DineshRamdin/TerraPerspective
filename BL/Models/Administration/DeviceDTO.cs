using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class DeviceDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public string Type { get; set; }
        public string DefaultCarousel { get; set; }
    }

    public class DeviceCRUDDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public int Type { get; set; }
        public long DefaultCarousel { get; set; }
    }
}
