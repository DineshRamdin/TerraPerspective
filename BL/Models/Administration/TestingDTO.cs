using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class TestingDTO
    {
        public long Id { get; set; }

        public string TestingName { get; set; }
        public string TestingType { get; set; }
        public string TestingDate { get; set; }
        public string? TestingCode { get; set; }
        public string? Description { get; set; }

    }

    public class TestingCRUDDTO
    {
        public long Id { get; set; }
        public string TestingName { get; set; }
        public int TestingType { get; set; }
        public string TestingDate { get; set; }
        public string? TestingCode { get; set; }
        public string? Description { get; set; }

       
    }
}
