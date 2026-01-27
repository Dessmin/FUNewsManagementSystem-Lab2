using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRN232.FUNewsManagementSystem.Services.Models.Business;

public class AuthService : IAuthService
{
    private readonly ILogger _loggerService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> loggerService)
    {
        _unitOfWork = unitOfWork;
        _loggerService = loggerService;
    }

    /// <summary>
    ///     Register a new user.
    /// </summary>
    /// <param name="registrationModel"></param>
    /// <returns></returns>
    public async Task<UserModel?> RegisterUserAsync(UserRegistrationModel registrationModel)
    {
        _loggerService.LogInformation($"Start registration for {registrationModel.AccountEmail}");

        if (await _unitOfWork.AccountRepository.FirstOrDefaultAsync(u => u.AccountEmail == registrationModel.AccountEmail) != null)
        {
            _loggerService.LogWarning($"Email {registrationModel.AccountEmail} already registered.");
            throw ErrorHelper.Conflict("Email have been used.");
        }

        var hashedPassword = new PasswordHasher<SystemAccount>().HashPassword(null, registrationModel.Password);

        var user = new SystemAccount
        {
            AccountName = registrationModel.AccountName,
            AccountEmail = registrationModel.AccountEmail,
            AccountPassword = hashedPassword,
            AccountRole = registrationModel.AccountRole
        };

        await _unitOfWork.AccountRepository.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _loggerService.LogInformation($"User {user.AccountEmail} created successfully.");

        return new UserModel
        {
            AccountID = user.AccountID,
            AccountName = user.AccountName,
            AccountEmail = user.AccountEmail,
            AccountRole = user.AccountRole
        };
    }

    /// <summary>
    ///     Login a user and return JWT access and refresh token.
    /// </summary>
    /// <param name="loginModel"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public async Task<AuthResultModel?> LoginAsync(LoginModel loginModel, IConfiguration configuration)
    {
        try
        {
            _loggerService.LogInformation($"Login attempt for {loginModel.Email}");

            // Get user from DB
            var user = await _unitOfWork.AccountRepository.FirstOrDefaultAsync(u => u.AccountEmail == loginModel.Email);

            if (user == null)
                throw ErrorHelper.NotFound("Account does not exist.");

            var verificationResult = new PasswordHasher<SystemAccount>().VerifyHashedPassword(null, user.AccountPassword, loginModel.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
                throw ErrorHelper.Unauthorized("Password is incorrect.");

            _loggerService.LogInformation($"User {loginModel.Email} authenticated successfully.");

            // Generate JWT token and refresh token
            var accessToken = JwtUtils.GenerateJwtToken(
                user.AccountID,
                user.AccountEmail ?? string.Empty,
                user.AccountRole.ToString(),
                configuration,
                TimeSpan.FromMinutes(30)
            );

            var refreshToken = Guid.NewGuid().ToString();

            _loggerService.LogInformation($"Tokens generated for {user.AccountEmail}");

            return new AuthResultModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Error during login for {loginModel.Email}: {ex.Message}");
            throw;
        }
    }
}

