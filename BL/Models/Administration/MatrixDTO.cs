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

		public class ZoneMatrixDTO
		{
            public long ZID { get; set; }
            public string ZAUID { get; set; }
            public List<long> IDS { get; set; } = new List<long>();
            public List<long> ParentIDS { get; set; } = new List<long>();
        }
	}

	public class MatrixListDTO
	{
		public long GMID { get; set; }
		public string GMDescription { get; set; }
	}

    public class CRUDMatrix
    {
        public string id { get; set; }
        public string text { get; set; }
        public string parent { get; set; }
        public state state { get; set; } = new state();
		public List<DropdownNodeNew> dropdownNodes { get; set; } = new List<DropdownNodeNew>();
    }

    public class DropdownNodeNew
    {
        //public string Value { get; set; }
        public string Title { get; set; }
        public bool Checked { get; set; }
        public string Href { get; set; }
        public List<DropdownNodeNew> Data { get; set; } = new List<DropdownNodeNew>();
        public List<DataAttr> DataAttrs { get; set; } = new List<DataAttr>();
    }

    public class ChildNodeNew
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public List<DataAttr> DataAttrs { get; set; } = new List<DataAttr>();
        public List<ChildNode> Children { get; set; } = new List<ChildNode>();
    }

    public class DataAttr
    {
        public string Title { get; set; }
        public string Data { get; set; }
    }


    public class DropdownNode
    {
        public string Value { get; set; } // Parent node ID
        public string Text { get; set; } // Parent node text
        public bool Checked { get; set; }
        public List<ChildNode> Children { get; set; } // Child nodes
    }

    public class ChildNode
    {
        public string Value { get; set; } // Child node ID
        public string Text { get; set; } // Child node text
        public bool Checked { get; set; }
    }

    public class ChildToChildNode
	{
		public string Id { get; set; } // Child node ID
		public string Text { get; set; } // Child node text
	}

	public class state
    {
        public bool Checked { get; set; } = false;
        public bool Opened { get; set; } = false;
    }
}
