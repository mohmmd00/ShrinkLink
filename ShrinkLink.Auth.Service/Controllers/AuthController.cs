using AuthService.Webapp.Contracts.Dtos;
using Microsoft.AspNetCore.Mvc;
using ShrinkLink.Auth.Service.Models.DataTransferObjects;
using ShrinkLink.Auth.Service.Models.Entities;
using ShrinkLink.Auth.Service.Services;

namespace ShrinkLink.Auth.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;

        public AuthController(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterTransferObject request, CancellationToken ct)
        {
            var response = await _authorizationService.Register(request, ct);
            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("username is already exists");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginTransferObject request, CancellationToken ct)
        {
            var tokenResponse = await _authorizationService.Login(request, ct);
            if (tokenResponse != null)
            {
                return Ok(tokenResponse);
            }
            else
            {
                return BadRequest("username or password is incorrect");
            }
        }

        [HttpGet("UserBaseDetails/{id}")]
        public async Task<UserBaseInformationTransferObject> GetUserById(string id, CancellationToken ct = default)
        {
            var fetchedUser = await _authorizationService.UserDetails(id, ct);

            var userBaseInfo = new UserBaseInformationTransferObject()
            {
                Id = fetchedUser.Id,
                Username = fetchedUser.Username
            };
            return userBaseInfo;
        }
    }
}
