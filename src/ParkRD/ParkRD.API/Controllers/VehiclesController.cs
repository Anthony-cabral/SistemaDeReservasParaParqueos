using Microsoft.AspNetCore.Mvc;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Models.Dtos;
using ParkRD.Domain.Entities;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly DataContext _context;

        public VehiclesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VehicleDto>> GetAll()
        {
            var vehicles = _context.Vehicles.Select(vehicle => new VehicleDto
            {
                Id = vehicle.Id,
                Plate = vehicle.Plate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Color = vehicle.Color,
                UserId = vehicle.UserId,
                IsActive = vehicle.IsActive
            }).ToList();

            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        public ActionResult<VehicleDto> GetById(int id)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(vehicle => vehicle.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            var response = new VehicleDto
            {
                Id = vehicle.Id,
                Plate = vehicle.Plate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Color = vehicle.Color,
                UserId = vehicle.UserId,
                IsActive = vehicle.IsActive
            };

            return Ok(response);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateVehicleDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Plate))
            {
                return BadRequest("The plate is required.");
            }

            if (request.UserId <= 0)
            {
                return BadRequest("You need to select a user.");
            }

            var user = _context.Users.FirstOrDefault(user => user.Id == request.UserId);

            if (user == null)
            {
                return BadRequest("The selected user was not found.");
            }

            var plateExists = _context.Vehicles.Any(vehicle => vehicle.Plate == request.Plate);

            if (plateExists)
            {
                return BadRequest("This plate is already registered.");
            }

            var vehicle = new Vehicles
            {
                Plate = request.Plate,
                Brand = request.Brand,
                Model = request.Model,
                Color = request.Color,
                UserId = request.UserId,
                IsActive = true
            };

            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            return Ok(new { Id = vehicle.Id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateVehicleDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("The selected vehicle does not match the information sent.");
            }

            var existing = _context.Vehicles.FirstOrDefault(vehicle => vehicle.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            if (request.UserId <= 0)
            {
                return BadRequest("You need to select a user.");
            }

            var user = _context.Users.FirstOrDefault(user => user.Id == request.UserId);

            if (user == null)
            {
                return BadRequest("The selected user was not found.");
            }

            var plateExists = _context.Vehicles.Any(vehicle => vehicle.Id != id && vehicle.Plate == request.Plate);

            if (plateExists)
            {
                return BadRequest("This plate is already registered.");
            }

            existing.Plate = request.Plate;
            existing.Brand = request.Brand;
            existing.Model = request.Model;
            existing.Color = request.Color;
            existing.UserId = request.UserId;
            existing.IsActive = request.IsActive;

            _context.Vehicles.Update(existing);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _context.Vehicles.FirstOrDefault(vehicle => vehicle.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            _context.Vehicles.Remove(existing);
            _context.SaveChanges();

            return NoContent();
        }
    }
}