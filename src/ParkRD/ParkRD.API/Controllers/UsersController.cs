using Microsoft.AspNetCore.Mvc;
using ParkRD.API.Data;
using ParkRD.API.Models.Dtos;
using ParkRD.API.Models.Entities;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAll()
        {
            var users = _context.Users.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalId = user.NationalId,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }).ToList();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDto> GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var response = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalId = user.NationalId,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return BadRequest("FirstName is required.");
            }

            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest("LastName is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NationalId))
            {
                return BadRequest("NationalId is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var emailExists = _context.Users.Any(user => user.Email == request.Email);

            if (emailExists)
            {
                return BadRequest("Email is already registered.");
            }

            var nationalIdExists = _context.Users.Any(user => user.NationalId == request.NationalId);

            if (nationalIdExists)
            {
                return BadRequest("NationalId is already registered.");
            }

            var user = new Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                NationalId = request.NationalId,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { Id = user.Id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateUserDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            var existing = _context.Users.FirstOrDefault(user => user.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            var emailExists = _context.Users.Any(user => user.Id != id && user.Email == request.Email);

            if (emailExists)
            {
                return BadRequest("Email is already registered.");
            }

            var nationalIdExists = _context.Users.Any(user => user.Id != id && user.NationalId == request.NationalId);

            if (nationalIdExists)
            {
                return BadRequest("NationalId is already registered.");
            }

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.NationalId = request.NationalId;
            existing.Email = request.Email;
            existing.PhoneNumber = request.PhoneNumber;
            existing.IsActive = request.IsActive;

            _context.Users.Update(existing);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _context.Users.FirstOrDefault(user => user.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            _context.Users.Remove(existing);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
