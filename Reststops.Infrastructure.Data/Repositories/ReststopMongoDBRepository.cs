using System;
using System.Linq;
using AutoMapper;
using Reststops.Core.Interfaces.Repositories;
using System.Threading.Tasks;
using Reststops.Core.Entities;
using Reststops.Infrastructure.Data.DAO;
using MongoDB.Driver;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using NetTopologySuite.Geometries;
using Reststops.Core.Extensions;

namespace Reststops.Infrastructure.Repositories
{
    public class ReststopMongoDBRepository : IReststopRepository
    {
        private readonly IMapper _mapper;
        private readonly IMongoClient _mongoClient;

        private const string DbName = "reststops";
        private const string ColName = "reststops";

        public ReststopMongoDBRepository(
            IMongoClient mongoClient,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
        }

        #region Public Methods

        public async Task Insert(Reststop reststop)
        {
            ReststopDAO dao = _mapper.Map<ReststopDAO>(reststop);
            
            var database = _mongoClient.GetDatabase (DbName);
            var collection = database.GetCollection<ReststopDAO> (ColName);

            await collection.InsertOneAsync(dao);
        }

        public async Task<List<Reststop>> GetWithinPolygon(Coordinate currentLocation, Polygon polygon)
        {
            var database = _mongoClient.GetDatabase (DbName);
            var collection = database.GetCollection<ReststopDAO> (ColName);
            
            var filter = Builders<ReststopDAO>.Filter.GeoWithinPolygon(
                p => p.Location, 
                GetCoordinatesArray(polygon));
            
            List<ReststopDAO> results = await collection.Find(filter).ToListAsync();

            return _mapper.Map<List<Reststop>>(results);
            return await Task.FromResult(new List<Reststop>());
        }

        #endregion

        #region Private Methods

        private static double[,] GetCoordinatesArray(Polygon polygon)
            => polygon.Coordinates
                .Select(c => GetCoordinateArray(c))
                .To2DArray();

        private static double[] GetCoordinateArray(Coordinate c)
            => new double[] {
                Math.Round(c.X, 5),
                Math.Round(c.Y, 5)
            };
        
        #endregion
    }
}
