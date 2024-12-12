using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string Surname { get; set; }
        public string Othername { get; set; }
        public Guid UserToken { get; set; }
        public Guid OTT { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }

		public bool FirstTimeLogin { get; set; } = true;

		public virtual LoginLog? LoginDetail { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Boolean DeleteStatus { get; set; }
		public int Company { get; set; } = 0;

		[NotMapped]
        public bool Logout { get; set; }

        [NotMapped]
        public string RoleId { get; set; }

        [NotMapped]
        public string Role { get; set; }

        [NotMapped]
        public string ProfileImage { get; set; }
       
        [NotMapped]
        public string GlobalParamValue { get; set; }

        
    }

	public class TunnelApplicationUser
	{

		public string Title { get; set; }
		public string Email { get; set; }
		public string NormEmail { get; set; }
		public string Username { get; set; }
		public string NormUsername { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string FkID { get; set; }

	}
	public enum UserStatus
    {
        Active,
        Suspended,
        Terminated,
        PasswordExpired,
        AccountLocked,
        ChangePassword
    }
}
