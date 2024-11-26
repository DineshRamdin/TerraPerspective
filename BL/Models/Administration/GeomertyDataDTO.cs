using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class GeomertyDataDTO
    {
        public long Id { get; set; }
        public string Zone { get; set; }
        public string FeatureGeoJson { get; set; }

    }
}
