using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Reststops.Infrastructure.Repositories
{
    public class CosmosDocumentRepository
    {
        protected readonly IDocumentClient _cosmosClient;

        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _partitionKey;
        private readonly string _spatialKey;

        private DocumentCollection _collection;

        public CosmosDocumentRepository(
            IDocumentClient cosmosClient,
            string databaseName,
            string collectionName,
            string partitionKey,
            string spatialKey = null
        )
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            _partitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
            _spatialKey = spatialKey;
        }

        protected async Task<DocumentCollection> GetDatabaseCollection()
        {
            if (_collection != null) return _collection;

            await _cosmosClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName });

            var partitionKey = new PartitionKeyDefinition();
            partitionKey.Paths.Add(_partitionKey);

            var indexingPolicy = new IndexingPolicy
            {
                Automatic = true
            };

            if (!string.IsNullOrWhiteSpace(_spatialKey))
            {
                // Add a spatial index including the required boundingBox
                SpatialSpec spatialPath = new SpatialSpec
                {
                    Path = _spatialKey
                };

                spatialPath.SpatialTypes.Add(SpatialType.Point);

                indexingPolicy.SpatialIndexes.Add(spatialPath);
            }

            _collection = await _cosmosClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(_databaseName),
                new DocumentCollection
                {
                    Id = _collectionName,
                    PartitionKey = partitionKey,
                    IndexingPolicy = indexingPolicy
                }
            ); ;

            return _collection;
        }

        protected async static Task<List<T>> GetAllResults<T>(IDocumentQuery<T> queryAll)
        {
            var results = new List<T>();

            while (queryAll.HasMoreResults)
            {
                var docs = await queryAll.ExecuteNextAsync<T>();
                results.AddRange(docs);
            }

            return results;
        }
    }
}
