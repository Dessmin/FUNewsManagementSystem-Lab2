using Microsoft.Extensions.Configuration;
using PRN232.FUNewsManagementSystem.Services.Models.Business;

public interface IAuthService
{
    Task<UserModel?> RegisterUserAsync(UserRegistrationModel registrationModel);
    Task<AuthResultModel?> LoginAsync(LoginModel loginModel, IConfiguration configuration);
}



