using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.Business;
using Template.Business.DTOs;
using Template.Business.Services;

namespace Template.WebAPI.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDto>> GetUser(Guid userId)
        {
            var userDto = await _userService.GetUser(userId);
            if (userDto is null)
                return NotFound();
            return Ok(userDto);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await _userService.DeleteUser(userId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto user)
        {
            try
            {
                await  _userService.CreateUser(user);
                return Created();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        
    }
}