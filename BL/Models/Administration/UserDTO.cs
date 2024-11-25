using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class UserDTO
    {
        public string Id { get; set; }


        public string Surname { get; set; }
        public string Othername { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? RoleName { get; set; }
        public string? ProfileImagebase64 { get; set; }
    }
}
