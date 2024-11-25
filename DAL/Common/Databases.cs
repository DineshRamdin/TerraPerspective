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
}
