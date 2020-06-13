using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Reststops.Core.Interfaces.Repositories;
using Reststops.Core.Interfaces.Services;
using Reststops.Core.Entities;
using Reststops.Presentation.API.Models;

namespace Reststops.Presentation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReststopsController : ControllerBase
    {
        private readonly ILogger<ReststopsController> _logger;
        private readonly IMapper _mapper;
        private readonly IReststopRepository _reststopRepository;
        private readonly INavigationService _navigationService;

        public ReststopsController(
            ILogger<ReststopsController> logger,
            IMapper mapper,
            IReststopRepository reststopRepository,
            INavigationService navigationService
        )
        {
            _logger = logger;
            _mapper = mapper;

            _reststopRepository = reststopRepository
                ?? throw new ArgumentNullException(nameof(reststopRepository));

            _navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReststopModel>>> Get(
            [FromQuery] double startLon,
            [FromQuery] double startLat,
            [FromQuery] double endLon,
            [FromQuery] double endLat
        )
        {
            var startCoordinate = new Coordinate(startLon, startLat);
            var endCoordinate = new Coordinate(endLon, endLat);

            DirectionsRoute route = await _navigationService.GetDirections(
                startCoordinate,
                endCoordinate
            );

            if (route == null || route.Routes.Count == 0)
            {
                return NotFound("DirectionsRoute not found");
            }

            Polygon bufferedPolygon = GetBufferedPolygonFromDirectionsRouteGeometry(route);

            IEnumerable<Reststop> reststops = await _reststopRepository
                .GetWithinPolygon(
                    startCoordinate,
                    bufferedPolygon
                );

            DirectionsMatrix matrix = await _navigationService.GetMatrix(
                startCoordinate,
                reststops.Select(r => new Coordinate(r.Longitude, r.Latitude))
            );

            // map entities to api models
            var reststopModels = _mapper.Map<IEnumerable<ReststopModel>>(
                reststops
            );

            reststopModels = AddDistanceAndDurationValuesToModels(matrix, reststopModels);
            reststopModels = reststopModels.OrderBy(m => m.DistanceInMeters);

            return Ok(new ReststopsModel
            {
                Reststops = reststopModels,
                Route = EncodedPolyline.Encode(route.Routes[0].Geometry.ToCoordinates()),
                Corridor = EncodedPolyline.Encode(bufferedPolygon.Coordinates)
            });
        }

        private static IEnumerable<ReststopModel> AddDistanceAndDurationValuesToModels(
            DirectionsMatrix matrix,
            IEnumerable<ReststopModel> models
        )
            => models.Select((ReststopModel m, int index) => m
                .WithDistanceAndDuration(
                    matrix.Distances[0][index],
                    matrix.Durations[0][index]
                )
            );

        private static Polygon GetBufferedPolygonFromDirectionsRouteGeometry(DirectionsRoute route)
            => new LineString(
                route.Routes[0].Geometry.Coordinates
                    .Select(c => new Coordinate(c[0], c[1]))
                    .ToArray()
                )
                .Buffer(0.02)
                .Reverse() as Polygon;
    }
}
