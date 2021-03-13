using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Reststops.Core.Interfaces.Services;
using Reststops.Presentation.API.Models;

namespace Reststops.Presentation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlacesController : ControllerBase
    {
        private const string InvalidTextParameterError = "Invalid 'text' parameter specified";

        private readonly IMapper _mapper;
        private readonly IGeocodingService _geocodingService;

        public PlacesController(
            IMapper mapper,
            IGeocodingService geocodingService
        )
        {
            _mapper = mapper;
            _geocodingService = geocodingService ?? throw new ArgumentNullException(nameof(geocodingService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaceModel>>> Get([FromQuery] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest(InvalidTextParameterError);
            }

            var geocodingResult = await _geocodingService.GetForwardGeocoding(text);

            return Ok(
                _mapper.Map<IEnumerable<PlaceModel>>(geocodingResult.Features)
            );
        }
    }
}
