using Microsoft.AspNetCore.Mvc;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Models.Dtos;
using ParkRD.Domain.Entities;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly DataContext _context;

        public ReservationsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReservationDto>> GetAll()
        {
            var reservations = _context.Reservations.Select(reservation => new ReservationDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                VehicleId = reservation.VehicleId,
                ParkingId = reservation.ParkingId,
                ReservationDate = reservation.ReservationDate,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ReservationType = reservation.ReservationType,
                Status = reservation.Status,
                TotalAmount = reservation.TotalAmount,
                CreatedAt = reservation.CreatedAt
            }).ToList();

            return Ok(reservations);
        }

        [HttpGet]
        [Route("active")]
        public ActionResult<IEnumerable<ReservationDto>> GetActive()
        {
            var reservations = _context.Reservations
                .Where(reservation => reservation.Status == "Reserved")
                .Select(reservation => new ReservationDto
                {
                    Id = reservation.Id,
                    UserId = reservation.UserId,
                    VehicleId = reservation.VehicleId,
                    ParkingId = reservation.ParkingId,
                    ReservationDate = reservation.ReservationDate,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    ReservationType = reservation.ReservationType,
                    Status = reservation.Status,
                    TotalAmount = reservation.TotalAmount,
                    CreatedAt = reservation.CreatedAt
                }).ToList();

            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public ActionResult<ReservationDto> GetById(int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(reservation => reservation.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var response = new ReservationDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                VehicleId = reservation.VehicleId,
                ParkingId = reservation.ParkingId,
                ReservationDate = reservation.ReservationDate,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ReservationType = reservation.ReservationType,
                Status = reservation.Status,
                TotalAmount = reservation.TotalAmount,
                CreatedAt = reservation.CreatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateReservationDto request)
        {
            if (request.UserId <= 0)
            {
                return BadRequest("You need to select a user.");
            }

            if (request.VehicleId <= 0)
            {
                return BadRequest("You need to select a vehicle.");
            }

            if (request.ParkingId <= 0)
            {
                return BadRequest("You need to select a parking space.");
            }

            if (request.EndTime <= request.StartTime)
            {
                return BadRequest("The end time cannot be before the start time.");
            }

            var user = _context.Users.FirstOrDefault(user => user.Id == request.UserId);

            if (user == null)
            {
                return BadRequest("The selected user was not found.");
            }

            var vehicle = _context.Vehicles.FirstOrDefault(vehicle => vehicle.Id == request.VehicleId);

            if (vehicle == null)
            {
                return BadRequest("The selected vehicle was not found.");
            }

            if (vehicle.UserId != request.UserId)
            {
                return BadRequest("This vehicle does not belong to the selected user.");
            }

            var parking = _context.Parkings.FirstOrDefault(parking => parking.Id == request.ParkingId);

            if (parking == null)
            {
                return BadRequest("The selected parking space was not found.");
            }

            var reservationExists = _context.Reservations.Any(reservation =>
                reservation.ParkingId == request.ParkingId &&
                reservation.ReservationDate.Date == request.ReservationDate.Date &&
                reservation.Status != "Cancelled" &&
                request.StartTime < reservation.EndTime &&
                request.EndTime > reservation.StartTime
            );

            if (reservationExists)
            {
                return BadRequest("This parking space is already reserved for that time.");
            }

            decimal totalAmount;

            if (request.ReservationType == "Day")
            {
                totalAmount = parking.DailyRate;
            }
            else
            {
                var hours = (decimal)(request.EndTime - request.StartTime).TotalHours;
                totalAmount = hours * parking.HourlyRate;
            }

            var reservation = new Reservations
            {
                UserId = request.UserId,
                VehicleId = request.VehicleId,
                ParkingId = request.ParkingId,
                ReservationDate = request.ReservationDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ReservationType = request.ReservationType,
                Status = "Reserved",
                TotalAmount = totalAmount,
                CreatedAt = DateTime.Now
            };

            parking.Status = "Reserved";

            _context.Reservations.Add(reservation);
            _context.Parkings.Update(parking);
            _context.SaveChanges();

            return Ok(new { Id = reservation.Id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateReservationDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("The selected reservation does not match the information sent.");
            }

            var existing = _context.Reservations.FirstOrDefault(reservation => reservation.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            if (request.UserId <= 0)
            {
                return BadRequest("You need to select a user.");
            }

            if (request.VehicleId <= 0)
            {
                return BadRequest("You need to select a vehicle.");
            }

            if (request.ParkingId <= 0)
            {
                return BadRequest("You need to select a parking space.");
            }

            if (request.EndTime <= request.StartTime)
            {
                return BadRequest("The end time cannot be before the start time.");
            }

            var user = _context.Users.FirstOrDefault(user => user.Id == request.UserId);

            if (user == null)
            {
                return BadRequest("The selected user was not found.");
            }

            var vehicle = _context.Vehicles.FirstOrDefault(vehicle => vehicle.Id == request.VehicleId);

            if (vehicle == null)
            {
                return BadRequest("The selected vehicle was not found.");
            }

            if (vehicle.UserId != request.UserId)
            {
                return BadRequest("This vehicle does not belong to the selected user.");
            }

            var parking = _context.Parkings.FirstOrDefault(parking => parking.Id == request.ParkingId);

            if (parking == null)
            {
                return BadRequest("The selected parking space was not found.");
            }

            var reservationExists = _context.Reservations.Any(reservation =>
                reservation.Id != id &&
                reservation.ParkingId == request.ParkingId &&
                reservation.ReservationDate.Date == request.ReservationDate.Date &&
                reservation.Status != "Cancelled" &&
                request.StartTime < reservation.EndTime &&
                request.EndTime > reservation.StartTime
            );

            if (reservationExists)
            {
                return BadRequest("This parking space is already reserved for that time.");
            }

            decimal totalAmount;

            if (request.ReservationType == "Day")
            {
                totalAmount = parking.DailyRate;
            }
            else
            {
                var hours = (decimal)(request.EndTime - request.StartTime).TotalHours;
                totalAmount = hours * parking.HourlyRate;
            }

            existing.UserId = request.UserId;
            existing.VehicleId = request.VehicleId;
            existing.ParkingId = request.ParkingId;
            existing.ReservationDate = request.ReservationDate;
            existing.StartTime = request.StartTime;
            existing.EndTime = request.EndTime;
            existing.ReservationType = request.ReservationType;
            existing.Status = request.Status;
            existing.TotalAmount = totalAmount;

            _context.Reservations.Update(existing);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _context.Reservations.FirstOrDefault(reservation => reservation.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(existing);
            _context.SaveChanges();

            return NoContent();
        }
    }
}