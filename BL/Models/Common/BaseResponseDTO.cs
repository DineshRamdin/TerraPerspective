﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Common
{
    public class BaseResponseDTO<T>
    {

        public T? Data { get; set; }
        public Guid oneTimeToken { get; set; }
        public Guid RoleId { get; set; }
        public string Role { get; set; }
        public string? ErrorMessage { get; set; }
        public string? QryResult { get; set; }
    }
}