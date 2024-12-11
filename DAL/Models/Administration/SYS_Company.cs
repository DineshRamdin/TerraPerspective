using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
	public class SYS_Company : BaseAuditModel, IAuditable
	{
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }

        [StringLength(255, ErrorMessage = "Registration Number cannot exceed 255 characters. ")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Name of Company cannot exceed 255 characters. ")]
        public string NameofCompany { get; set; } = String.Empty;
        public DateTime? RegistrationDate { get; set; } = DateTime.Now;

        [StringLength(20, ErrorMessage = "TelephoneNo cannot exceed 20 characters. ")]
        public string TelephoneNumber { get; set; }

        [StringLength(20, ErrorMessage = "MobileNo cannot exceed 20 characters. ")]
        public string MobileNumber { get; set; }

        [StringLength(120, ErrorMessage = "Email cannot exceed 120 characters. ")]
        public string? Email { get; set; } = String.Empty;
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


    }
}
