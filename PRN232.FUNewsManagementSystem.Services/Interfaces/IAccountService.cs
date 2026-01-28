using PRN232.FUNewsManagementSystem.Services.Models.Account;

namespace PRN232.FUNewsManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for SystemAccount operations
/// </summary>
public interface IAccountService
{
    Task<AccountModel?> GetAccountByIdAsync(int id);
    Task<Pagination<AccountModel>> GetAccountsAsync(AccountQueryModel query);
    Task<AccountModel> CreateAccountAsync(CreateAccountModel model);
    Task<AccountModel> UpdateAccountAsync(int id, UpdateAccountModel model);
    Task<bool> DeleteAccountAsync(int id);
}
