﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
	public class SYS_SystemIcon : BaseAuditModel, IAuditable
	{
		[Key]
		public long Id { get; set; }		
		public string Name { get; set; }		
        public string? SystemIconImagebase64 { get; set; }


    }
}