using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Common
{
    public class Databases
    {
        public static readonly string PerspectiveConnection = "PerspectiveConnection";

    }

    public enum PosterType
    {
        HS = 1,
        WARN = 2,
        INFO = 3,
    }

    public enum UserType
    {
        User = 1,
        Resource = 2,
    }

    public enum DeviceType
    {
        PC = 1,
        ThinClient = 2,
        TV = 3,
    }

    public enum TestingType
    {
        Testing1 = 1,
        Testing2 = 2,
        Testing3 = 3,
    }

    public enum AccessOperationType
    {
        Create = 1,
        Read = 2,
        Update = 3,
        Delete = 4,
        Approve = 5,
        Upload = 6,
        Export = 7,
        Report = 8

    }

	public enum RowStructType
	{
		Role = 1,
		User = 2,
	}

	public enum ModuleName
	{
		ResearchCropPOW = 1,
		ResearchLiveStockPOW = 2,
		ResearchCropLR = 3,
		ResearchLiveStockLR = 4,
		ResearchCropTM = 5,
		ResearchLiveStockTM = 6,
	}

}
