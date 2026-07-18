using Microsoft.AspNetCore.Mvc;
using ParkRD.Application.Contract;
using ParkRD.Application.Dtos;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VehicleDto>> GetAll()
        {
            var result = _vehicleService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public ActionResult<VehicleDto> GetById(int id)
        {
            var result = _vehicleService.GetById(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateVehicleDto request)
        {
            var result = _vehicleService.Create(request);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Id = result.Data });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateVehicleDto request)
        {
            var result = _vehicleService.Update(id, request);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result.Message);
                }

                return BadRequest(result.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _vehicleService.Delete(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return NoContent();
        }
    }
}
