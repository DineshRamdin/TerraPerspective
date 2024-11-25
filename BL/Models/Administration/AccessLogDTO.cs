using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class AccessLogDTO
    {
        public long Id { get; set; }
        public string RoomName { get; set; }
        public string ResourceName { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Purpose { get; set; }
        public string Remarks { get; set; }
        public string Visitor { get; set; }
    }

    public class CheckInAccessLogDTO
    {
        public long Id { get; set; }
        public long Room { get; set; }
        public long Resource { get; set; }
        public string Password{ get; set; }
        public long? Purpose { get; set; }
        public string Remarks { get; set; }
        public string Visitor { get; set; }
    }

    public class CheckOutAccessLogDTO
    {
        public long Id { get; set; }
    }
}
