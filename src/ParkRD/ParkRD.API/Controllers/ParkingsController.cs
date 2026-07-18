using Microsoft.AspNetCore.Mvc;
using ParkRD.Application.Contract;
using ParkRD.Application.Dtos;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/parkings")]
    public class ParkingsController : ControllerBase
    {
        private readonly IParkingService _parkingService;

        public ParkingsController(IParkingService parkingService)
        {
            _parkingService = parkingService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ParkingDto>> GetAll()
        {
            var result = _parkingService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("available")]
        public ActionResult<IEnumerable<ParkingDto>> GetAvailable()
        {
            var result = _parkingService.GetAvailable();

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public ActionResult<ParkingDto> GetById(int id)
        {
            var result = _parkingService.GetById(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateParkingDto request)
        {
            var result = _parkingService.Create(request);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Id = result.Data });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateParkingDto request)
        {
            var result = _parkingService.Update(id, request);

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
            var result = _parkingService.Delete(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return NoContent();
        }
    }
}
