using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Reststops.Core.Entities
{
    public record Geometry
    {
        public double[][] Coordinates { get; init; }
        public string Type { get; init; }

        public IEnumerable<Coordinate> ToCoordinates()
            => Coordinates.Select(c => new Coordinate(c[0], c[1]));
    }
}
