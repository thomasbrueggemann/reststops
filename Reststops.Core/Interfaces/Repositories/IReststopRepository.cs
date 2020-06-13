using System.Collections.Generic;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using Reststops.Core.Entities;

namespace Reststops.Core.Interfaces.Repositories
{
    public interface IReststopRepository
    {
        public Task Insert(Reststop reststop);
        public Task<List<Reststop>> GetWithinPolygon(Coordinate currentLocation, Polygon polygon);
    }
}
