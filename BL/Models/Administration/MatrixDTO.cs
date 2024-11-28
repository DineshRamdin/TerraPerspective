using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
	public class MatrixDTO
	{
		public class UserMatrixDTO
		{
			public long UID { get; set; }
			public string AUID { get; set; }
			public List<long> IDS { get; set; } = new List<long>();
			public List<long> ParentIDS { get; set; } = new List<long>();

		}
	}

	public class MatrixListDTO
	{
		public long GMID { get; set; }
		public string GMDescription { get; set; }
	}
}
