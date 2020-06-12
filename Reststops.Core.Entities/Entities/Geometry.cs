using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Reststops.Domain.Entities
{
    public class Geometry
    {
        public double[][] Coordinates { get; set; }
        public string Type { get; set; }

        public IEnumerable<Coordinate> ToCoordinates()
            => Coordinates.Select(c => new Coordinate(c[0], c[1]));
    }
}
