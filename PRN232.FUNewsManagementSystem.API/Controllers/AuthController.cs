using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.FUNewsManagementSystem.API.Models.Request.Auth;
using PRN232.FUNewsManagementSystem.API.Models.Response.Auth;
using PRN232.FUNewsManagementSystem.Services.Models.Business;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.FUNewsManagementSystem.API.Controllers
{
    /// <summary>
    /// Authentication and authorization endpoints
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">User registration information</param>
        /// <returns>Registered user information</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Invalid request data or email already exists</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new user account with the provided registration information. Email must be unique."
        )]
        [ProducesResponseType(typeof(ApiResult<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<UserResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<UserResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            try
            {
                // Map Request Model to Business Model
                var registrationModel = new UserRegistrationModel
                {
                    AccountName = request.AccountName,
                    AccountEmail = request.Email,
                    Password = request.Password,
                    AccountRole = request.AccountRole
                };

                // Call Service with Business Model
                var userModel = await _authService.RegisterUserAsync(registrationModel);

                // Map Business Model to Response Model
                var response = new UserResponse
                {
                    AccountID = userModel!.AccountID,
                    AccountName = userModel.AccountName,
                    AccountEmail = userModel.AccountEmail,
                    AccountRole = userModel.AccountRole
                };

                return Ok(ApiResult<UserResponse>.Success(response, "200", "User registered successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<UserResponse>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        /// <summary>
        /// Authenticate user and obtain JWT tokens
        /// </summary>
        /// <param name="request">Login credentials (email and password)</param>
        /// <returns>JWT access token and refresh token</returns>
        /// <response code="200">Login successful, returns access and refresh tokens</response>
        /// <response code="400">Invalid request format</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">Account is disabled or not verified</response>
        /// <response code="404">Account not found</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "User login",
            Description = "Authenticate user with email and password. Returns JWT access token and refresh token."
        )]
        [ProducesResponseType(typeof(ApiResult<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<LoginResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<LoginResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<LoginResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<LoginResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Map Request Model to Business Model
            try
            {
                var loginModel = new LoginModel
                {
                    Email = request.Email,
                    Password = request.Password
                };

                // Call Service with Business Model
                var authResult = await _authService.LoginAsync(loginModel, _configuration);

                // Map Business Model to Response Model
                var response = new LoginResponse
                {
                    AccessToken = authResult!.AccessToken
                };

                return Ok(ApiResult<LoginResponse>.Success(response, "200", "Login successful."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<UserResponse>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
    }
}
