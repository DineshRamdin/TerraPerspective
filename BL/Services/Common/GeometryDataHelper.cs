using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class GeometryDataHelper
    {
        public static bool IsGeometryDataCounterClockwise(Polygon polygon)
        {
            var ring = polygon.ExteriorRing.Coordinates;
            double sum = 0.0;

            for (int i = 0; i < ring.Length - 1; i++)
            {
                var p1 = ring[i];
                var p2 = ring[i + 1];
                sum += (p2.X - p1.X) * (p2.Y + p1.Y);
            }

            // Return true if counter-clockwise (sum < 0)
            return sum < 0;
        }

        public static Polygon GeometryDataReversePolygon(Polygon polygon)
        {
            var reversedCoordinates = polygon.ExteriorRing.Coordinates.Reverse().ToArray();
            var geometryFactory = new GeometryFactory();
            return geometryFactory.CreatePolygon(reversedCoordinates);
        }

    }
}
