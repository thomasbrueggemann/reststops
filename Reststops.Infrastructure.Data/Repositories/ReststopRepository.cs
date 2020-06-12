using System;
using System.Linq;
using AutoMapper;
using Reststops.Core.Interfaces.Repositories;
using System.Threading.Tasks;
using Reststops.Domain.Entities;
using Reststops.Infrastructure.Data.DAO;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using NetTopologySuite.Geometries;

namespace Reststops.Infrastructure.Repositories
{
    public class ReststopRepository : CosmosDocumentRepository, IReststopRepository
    {
        private readonly IMapper _mapper;

        private const string DB_NAME = "Reststops";
        private const string COL_NAME = "Reststops";
        private const string PARTITION_KEY = "/id";
        private const string SPATIAL_KEY = "/location/*";

        public ReststopRepository(
            IDocumentClient cosmosClient,
            IMapper mapper
        ) : base(cosmosClient, DB_NAME, COL_NAME, PARTITION_KEY, SPATIAL_KEY)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Public Methods

        public async Task Insert(Reststop reststop)
        {
            ReststopDAO dao = _mapper.Map<ReststopDAO>(reststop);
            var collection = await GetDatabaseCollection();

            await _cosmosClient.CreateDocumentAsync(collection.DocumentsLink, dao);
        }

        public async Task<List<Reststop>> GetWithinPolygon(Coordinate currentLocation, Polygon polygon)
        {
            var collection = await GetDatabaseCollection();

            string queryText = GetWithinPolygonQueryString(currentLocation, polygon);

            var query =  _cosmosClient.CreateDocumentQuery<ReststopDAO>(
                collection.DocumentsLink, new SqlQuerySpec(queryText),
                new FeedOptions()
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = 25
                }
            ).AsDocumentQuery();

            List<ReststopDAO> results = await GetAllResults(query);

            return _mapper.Map<List<Reststop>>(results);
        }

        #endregion

        #region Private Methods

        private static double[][] GetCoordinatesArray(Polygon polygon)
            => polygon.Coordinates
                .Select(c => GetCoordinateArray(c))
                .ToArray();

        private static double[] GetCoordinateArray(Coordinate c)
            => new double[2] {
                Math.Round(c.X, 5),
                Math.Round(c.Y, 5)
            };

        private static string GetWithinPolygonQueryString(Coordinate currentLocation, Polygon polygon)
        {
            string currentLocationString = JsonConvert.SerializeObject(
                GetCoordinateArray(currentLocation)
            );

            string polygonString = JsonConvert.SerializeObject(
                GetCoordinatesArray(polygon).ToArray()
            );

            return "SELECT * FROM r WHERE " +
                                "ST_DISTANCE(r.location, { " +
                                    "'type': 'Point', 'coordinates':" + currentLocationString + "}) < 75000 AND " +
                                "ST_WITHIN(r.location, { " +
                                    "'type':'Polygon', 'coordinates': [" + polygonString + "]})";
        }

        #endregion
    }
}
