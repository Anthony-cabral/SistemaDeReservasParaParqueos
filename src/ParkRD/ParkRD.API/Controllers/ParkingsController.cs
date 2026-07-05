using Microsoft.AspNetCore.Mvc;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Models.Dtos;
using ParkRD.Domain.Entities;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/parkings")]
    public class ParkingsController : ControllerBase
    {
        private readonly DataContext _context;

        public ParkingsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ParkingDto>> GetAll()
        {
            var parkings = _context.Parkings.Select(parking => new ParkingDto
            {
                Id = parking.Id,
                Code = parking.Code,
                Status = parking.Status,
                HourlyRate = parking.HourlyRate,
                DailyRate = parking.DailyRate,
                IsActive = parking.IsActive
            }).ToList();

            return Ok(parkings);
        }

        [HttpGet]
        [Route("available")]
        public ActionResult<IEnumerable<ParkingDto>> GetAvailable()
        {
            var parkings = _context.Parkings
                .Where(parking => parking.Status == "Available" && parking.IsActive)
                .Select(parking => new ParkingDto
                {
                    Id = parking.Id,
                    Code = parking.Code,
                    Status = parking.Status,
                    HourlyRate = parking.HourlyRate,
                    DailyRate = parking.DailyRate,
                    IsActive = parking.IsActive
                }).ToList();

            return Ok(parkings);
        }

        [HttpGet("{id}")]
        public ActionResult<ParkingDto> GetById(int id)
        {
            var parking = _context.Parkings.FirstOrDefault(parking => parking.Id == id);

            if (parking == null)
            {
                return NotFound();
            }

            var response = new ParkingDto
            {
                Id = parking.Id,
                Code = parking.Code,
                Status = parking.Status,
                HourlyRate = parking.HourlyRate,
                DailyRate = parking.DailyRate,
                IsActive = parking.IsActive
            };

            return Ok(response);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateParkingDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest("The parking code is required.");
            }

            var codeExists = _context.Parkings.Any(parking => parking.Code == request.Code);

            if (codeExists)
            {
                return BadRequest("This parking code is already registered.");
            }

            var parking = new Parkings
            {
                Code = request.Code,
                Status = "Available",
                HourlyRate = request.HourlyRate,
                DailyRate = request.DailyRate,
                IsActive = true
            };

            _context.Parkings.Add(parking);
            _context.SaveChanges();

            return Ok(new { Id = parking.Id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateParkingDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("The selected parking space does not match the information sent.");
            }

            var existing = _context.Parkings.FirstOrDefault(parking => parking.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            var codeExists = _context.Parkings.Any(parking => parking.Id != id && parking.Code == request.Code);

            if (codeExists)
            {
                return BadRequest("This parking code is already registered.");
            }

            existing.Code = request.Code;
            existing.Status = request.Status;
            existing.HourlyRate = request.HourlyRate;
            existing.DailyRate = request.DailyRate;
            existing.IsActive = request.IsActive;

            _context.Parkings.Update(existing);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _context.Parkings.FirstOrDefault(parking => parking.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            _context.Parkings.Remove(existing);
            _context.SaveChanges();

            return NoContent();
        }
    }
}