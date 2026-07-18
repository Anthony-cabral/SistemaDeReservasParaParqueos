using Microsoft.AspNetCore.Mvc;
using ParkRD.Application.Contract;
using ParkRD.Application.Dtos;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAll()
        {
            var result = _userService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDto> GetById(int id)
        {
            var result = _userService.GetById(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public ActionResult<int> Create(CreateUserDto request)
        {
            var result = _userService.Create(request);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Id = result.Data });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateUserDto request)
        {
            var result = _userService.Update(id, request);

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
            var result = _userService.Delete(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return NoContent();
        }
    }
}
