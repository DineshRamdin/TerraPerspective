﻿using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ZoneManagementDTO
    {
        public long Id { get; set; }
        public string Zone { get; set; }
        public string FeatureGeoJson { get; set; }
        public string Type { get; set; }

        public Geometry geometry { get; set; }

        public string Folder { get; set; }
        public string ExternalReference { get; set; }

		public List<long> ZoneMatrix { get; set; } = new List<long>();

	}

    public class ZoneDataDTO
    {
        public long Id { get; set; }
        public string Zone { get; set; }
        public string Type { get; set; }
        public string Folder { get; set; }
        public string ExternalReference { get; set; }
    }
}
