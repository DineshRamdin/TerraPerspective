using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class CompanyDTO
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string NameofCompany { get; set; } = string.Empty;
        public DateTime? RegistrationDate { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; } = string.Empty;
        public long Locality { get; set; }
        public long? Country { get; set; }
        public string? PostalCode { get; set; }
        public long? MCAVCA { get; set; }
        public string? CompanyIcon { get; set; }
        public string? Follow1 { get; set; }
        public string? Follow2 { get; set; }
        public string? Follow3 { get; set; }
        public string? Follow4 { get; set; }
        public string? Follow5 { get; set; }
        public bool CompanyIconFlag { get; set; }


    }
}
