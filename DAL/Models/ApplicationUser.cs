﻿using Microsoft.AspNetCore.Identity;
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

        public virtual LoginLog? LoginDetail { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Boolean DeleteStatus { get; set; }

        [NotMapped]
        public bool Logout { get; set; }

        [NotMapped]
        public string RoleId { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }

    public enum UserStatus
    {
        Active,
        Suspended,
        Terminated,
        PasswordExpired,
        AccountLocked
    }
}