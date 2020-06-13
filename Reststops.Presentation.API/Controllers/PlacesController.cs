using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reststops.Core.Interfaces.Services;
using Reststops.Core.Entities;
using Reststops.Presentation.API.Models;

namespace Reststops.Presentation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly ILogger<PlacesController> _logger;
        private readonly IMapper _mapper;
        private readonly IGeocodingService _geocodingService;

        public PlacesController(
            ILogger<PlacesController> logger,
            IMapper mapper,
            IGeocodingService geocodingService
        )
        {
            _logger = logger;
            _mapper = mapper;

            _geocodingService = geocodingService
                ?? throw new ArgumentNullException(nameof(geocodingService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaceModel>>> Get(
            [FromQuery] string text
        )
        {
            ForwardGeocoding geocodingResult = await _geocodingService.GetForwardGeocoding(text);

            return Ok(
                _mapper.Map<IEnumerable<PlaceModel>>(geocodingResult.Features)
            );
        }
    }
}
