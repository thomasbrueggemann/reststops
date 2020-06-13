using System.Collections.Generic;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using Reststops.Core.Entities;

namespace Reststops.Core.Interfaces.Services
{
    public interface INavigationService
    {
        public Task<DirectionsRoute> GetDirections(
            IEnumerable<Coordinate> coordinates
        );
    }
}
