using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class SYS_Testing : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string TestingName { get; set; }
        public TestingType TestingType { get; set; }
        public string TestingDate { get; set; }
        public string TestingCode { get; set; }
        public string Description { get; set; }

    }
}
