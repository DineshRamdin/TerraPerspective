using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_ZoneManagement : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        public string Zone { get; set; }

        public Geometry GeomColumn { get; set; }
        public string Type { get; set; }
    }
}
