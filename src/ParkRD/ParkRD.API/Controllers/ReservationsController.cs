using Microsoft.AspNetCore.Mvc;
using ParkRD.Application.Contract;
using ParkRD.Application.Dtos;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReservationDto>> GetAll()
        {
            var result = _reservationService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("active")]
        public ActionResult<IEnumerable<ReservationDto>> GetActive()
        {
            var result = _reservationService.GetActive();

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public ActionResult<ReservationDto> GetById(int id)
        {
            var result = _reservationService.GetById(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateReservationDto request)
        {
            var result = _reservationService.Create(request);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Id = result.Data });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateReservationDto request)
        {
            var result = _reservationService.Update(id, request);

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
            var result = _reservationService.Delete(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return NoContent();
        }
    }
}
