using System.Linq;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using Simplify.NET;
using Point = Simplify.NET.Point;

namespace Reststops.Core.Entities
{
    public class Polyline
    {
        private IEnumerable<Coordinate> _coordinates { get; set; }

        public Polyline(IEnumerable<Coordinate> coordinates)
        {
            _coordinates = coordinates;
        }

        public IEnumerable<Coordinate> GetCoordinates() => _coordinates;

        public Polyline Simplify(double pixelTolerance = 5.0, bool highQuality = false)
        {
            var originalPoints = _coordinates
                .Select(c => new Point(c.X, c.Y))
                .ToList();

            var simplifiedList = SimplifyNet.Simplify(
                originalPoints,
                pixelTolerance,
                highQuality
            );

            return new Polyline(
                simplifiedList.Select(s => new Coordinate(s.X, s.Y))
            );
        }

        public Polyline FitEnvelope(Envelope envelope)
            => new Polyline(_coordinates.Where(c => envelope.Contains(c)));
    }
}
