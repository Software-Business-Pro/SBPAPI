using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SBPWebApi.Configuration;
using SBPWebApi.Models;
using SBPWebApi.Services;
using SBPWebApi.SwaggerExamples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;

namespace SBPWebApi.Controllers
{
    /// <summary>
    /// Foo controller.
    /// </summary>
    [Route("Api/[controller]")]
    public class SBPController : ControllerBase
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly ISBPService _service;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates new instance of <see cref="GoodtimeController"/>.
        /// </summary>
        /// <param name="connectionStrings">
        /// Instance of <see cref="IOptionsSnapshot{ConnectionStrings}"/> object that contains connection string.
        /// More information: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionssnapshot-1?view=aspnetcore-2.1
        /// </param>
        /// <param name="service">Instance of <see cref="ISBPService"/></param>
        /// <param name="logger"></param>
        public SBPController(IOptionsSnapshot<ConnectionStrings> connectionStrings, ISBPService service, ILogger<SBPController> logger)
        {
            _connectionStrings = connectionStrings.Value ?? throw new ArgumentNullException(nameof(connectionStrings));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service.SetConnectionString(_connectionStrings);
        }

        /// <summary>
        /// Tries to retrieve all Vehicule objects.
        /// </summary>
        /// <response code="200">All available Vehicule objects retrieved.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Vehicule"), ResponseCache(CacheProfileName = "default")]
        [ProducesResponseType(typeof(IEnumerable<Vehicule>), 200)]
        [SwaggerResponseExample(200, typeof(VehiculeListExemple))]
        public async Task<IActionResult> Get()
        {
            var response = await _service.GetAllVehicules().ConfigureAwait(false);
            return Ok(response);
        }

        /// <summary>
        /// Tries to retrieve planning of specified Vehicule.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <response code="200">planning successfully retrieved.</response>
        /// <response code="404">no planning for this vehicule.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Planning/{id}", Name = "getPlanById")]
        [ProducesResponseType(typeof(IEnumerable<Planning>), 200)]
        [ProducesResponseType(404)]
        [SwaggerResponseExample(200, typeof(PlanningExample))]
        public async Task<IActionResult> Get(string id)
        {
            var response = await _service.GetPlanning(id).ConfigureAwait(false);

            if (response == null)
                return NotFound(id);

            return Ok(response);
        }

        /// <summary>
        /// Tries to retrieve images of specified Vehicule.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <response code="200">images successfully retrieved.</response>
        /// <response code="404">no images for this vehicule.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Image/{id}", Name = "getImageById")]
        [ProducesResponseType(typeof(IEnumerable<Planning>), 200)]
        [ProducesResponseType(404)]
        [SwaggerResponseExample(200, typeof(LienImageExample))]
        public async Task<IActionResult> GetImage(string id)
        {
            var response = await _service.GetImageLinks(id).ConfigureAwait(false);

            if (response == null)
                return NotFound(id);

            return Ok(response);
        }

        /// <summary>
        /// Tries to upload a new Image for a vehicule.
        /// </summary>
        /// <param name="item">Instance of <see cref="IFormFile"/>.</param>
        /// <response code="200">Image uploaded.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Image/{id}")]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post(IFormFile file, string id)
        {
            _service.uploadImage(file, id);

            return Ok();
        }
    }
}