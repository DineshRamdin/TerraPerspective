using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
	public class TableCodeConfigurationDTO
	{
		public int Id { get; set; }
		public string TableName { get; set; }
		public string Prefix { get; set; }
		public string CompanyName { get; set; }
		public string ConfigurationName { get; set; }
		public bool? HasAddi { get; set; } = false;
		public string Comment { get; set; } = String.Empty;
	}

	public class TableCodeConfigurationCRUDDTO
	{
		public long Id { get; set; }
		public string TableName { get; set; }
		public string Prefix { get; set; }
		public int CompanyId { get; set; }
		public int ConfigurationId { get; set; }
		public bool? HasAddi { get; set; } = false;
		public string Comment { get; set; } = String.Empty;
	}
}
