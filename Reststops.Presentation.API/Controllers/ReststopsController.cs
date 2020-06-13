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
            [FromQuery] double endLat,
            [FromQuery] int maxDetourInSeconds = 300
        )
        {
            var startCoordinate = new Coordinate(startLon, startLat);
            var endCoordinate = new Coordinate(endLon, endLat);

            DirectionsRoute route = await _navigationService.GetDirections(
                new List<Coordinate>() { startCoordinate, endCoordinate}
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

            // map entities to api models
            var reststopModels = _mapper.Map<IEnumerable<ReststopModel>>(
                reststops
            );

            reststopModels = await AddDistanceAndDetourDurationToModels(
                reststopModels,
                startCoordinate,
                endCoordinate,
                route.Routes[0].Duration
            );

            reststopModels = reststopModels
                .Where(m => m.DetourDurationInSeconds <= maxDetourInSeconds)
                .OrderBy(m => m.DistanceInMeters);

            return Ok(new ReststopsModel
            {
                Reststops = reststopModels,
                Route = EncodedPolyline.Encode(route.Routes[0].Geometry.ToCoordinates()),
                Corridor = EncodedPolyline.Encode(bufferedPolygon.Coordinates)
            });
        }

        private async Task<IEnumerable<ReststopModel>> AddDistanceAndDetourDurationToModels(
            IEnumerable<ReststopModel> models,
            Coordinate startCoordinate,
            Coordinate endCoordinate,
            double originalRouteDurationInSeconds
        )
        {
            var routedModels = new List<ReststopModel>();

            foreach (ReststopModel model in models)
            {
                double distance = double.MaxValue;
                double detourDuration = double.MaxValue;

                // calculate route via this reststop to final destination
                DirectionsRoute route = await _navigationService.GetDirections(
                    new List<Coordinate>()
                    {
                        startCoordinate,
                        new Coordinate(model.Longitude, model.Latitude),
                        endCoordinate
                    }
                );

                if (route != null && route.Routes.Count > 0)
                {
                    distance = route.Routes[0].Legs[0].Distance;
                    detourDuration = Math.Abs(
                        route.Routes[0].Duration - originalRouteDurationInSeconds
                    );
                }

                routedModels.Add(
                    model.WithDistanceAndDetourDuration(
                        (int) Math.Round(distance),
                        (int) Math.Round(detourDuration)
                    )
                );
            }

            return routedModels;
        }

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
