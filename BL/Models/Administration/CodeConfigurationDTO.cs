using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
	public class CodeConfigurationDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Date { get; set; } 
		public string DateFormat { get; set; } = string.Empty;
		public string Month { get; set; }
		public string MonthFormat { get; set; } = string.Empty;
		public string Year { get; set; } 
		public string YearFormat { get; set; } = string.Empty;
		public string UsePrefix { get; set; } 
		public string Reset { get; set; }
		public string ResetConfig { get; set; } = string.Empty;
		public int PaddingNo { get; set; }
		public string Comment { get; set; } = String.Empty;
	}

	public class CodeConfigurationCRUDDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool Date { get; set; } = false;
		public string DateFormat { get; set; } = string.Empty;
		public bool Month { get; set; } = false;
		public string MonthFormat { get; set; } = string.Empty;
		public bool Year { get; set; } = false;
		public string YearFormat { get; set; } = string.Empty;
		public bool UsePrefix { get; set; } = false;
		public bool? Reset { get; set; } = false;
		public string ResetConfig { get; set; } = string.Empty;
		public int PaddingNo { get; set; }
		public string Comment { get; set; } = String.Empty;
	}
}
