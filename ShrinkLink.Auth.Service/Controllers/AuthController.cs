using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShrinkLink.Auth.Service.Models.Interfaces;
using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;


namespace ShrinkLink.Auth.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authorizationService;

        public AuthController(IAuthService authorizationService)
        {
            _authorizationService = authorizationService;
        }



        [HttpPost("/Register")]
        public async Task<IActionResult> Register([FromBody]RegisterRequestTransferObject request, CancellationToken ct)
        {
            var response = await _authorizationService.Register(request, ct);

            if (response == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred.",
                    Status = RegisterResultStatus.Failed
                });
            }
             // switch between statuss
            return response.Status switch
            {
                RegisterResultStatus.Success => Ok(response),
                RegisterResultStatus.UsernameExists => Conflict(response),
                RegisterResultStatus.EmailAddressExists => Conflict(response),
                RegisterResultStatus.InvalidUsername => BadRequest(response),
                RegisterResultStatus.InvalidPassword => BadRequest(response),
                RegisterResultStatus.InvalidEmailAddress => BadRequest(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred during registration."
                })
            };
        }







        [HttpPost("/Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestTransferObject request, CancellationToken ct)
        {
            var response = await _authorizationService.Login(request, ct);
            return response.Status switch
            {
                LoginResultStatus.Success => Ok(response),
                LoginResultStatus.InvalidUsernameOrPassword => Unauthorized(response),
                LoginResultStatus.InvalidInput => BadRequest(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }




        [HttpGet("/UserBasicDetails")]
        public async Task<IActionResult> GetUserBasicDetails(CancellationToken ct = default)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

            var userBasicInformation = await _authorizationService.UserBasicDetails(authHeader, ct);

            return userBasicInformation.Status switch
            {
                UserInfoResultStatus.Success => Ok(userBasicInformation),
                UserInfoResultStatus.InvalidToken => Unauthorized(userBasicInformation),
                _ => StatusCode(StatusCodes.Status500InternalServerError, userBasicInformation)
            };
        }

    }
}
