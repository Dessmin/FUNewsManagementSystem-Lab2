using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.Services.Services;

/// <summary>
/// Service implementation for SystemAccount operations
/// </summary>
public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    public async Task<AccountModel?> GetAccountByIdAsync(int id)
    {
        _logger.LogInformation($"Getting account with ID: {id}");

        var account = await _unitOfWork.AccountRepository.GetByIdAsync(id);

        if (account == null)
        {
            _logger.LogWarning($"Account with ID {id} not found");
            throw ErrorHelper.NotFound($"Account with ID {id} not found.");
        }

        return new AccountModel
        {
            AccountID = account.AccountID,
            AccountName = account.AccountName,
            AccountEmail = account.AccountEmail,
            AccountRole = account.AccountRole
        };
    }

    /// <summary>
    /// Get paginated list of accounts with search, filter, and sort
    /// </summary>
    public async Task<Pagination<AccountModel>> GetAccountsAsync(AccountQueryModel query)
    {
        _logger.LogInformation($"Getting accounts with query: Page={query.Page}, PageSize={query.PageSize}");

        // Get all accounts as queryable
        var accountsQuery = _unitOfWork.AccountRepository.GetAllAsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.ToLower();
            accountsQuery = accountsQuery.Where(a =>
                a.AccountName.ToLower().Contains(searchTerm) ||
                (a.AccountEmail != null && a.AccountEmail.ToLower().Contains(searchTerm)));
        }

        // Apply role filter
        if (query.AccountRole.HasValue)
        {
            accountsQuery = accountsQuery.Where(a => a.AccountRole == query.AccountRole.Value);
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            accountsQuery = query.SortBy.ToLower() switch
            {
                "name" => query.IsDescending
                    ? accountsQuery.OrderByDescending(a => a.AccountName)
                    : accountsQuery.OrderBy(a => a.AccountName),
                "email" => query.IsDescending
                    ? accountsQuery.OrderByDescending(a => a.AccountEmail)
                    : accountsQuery.OrderBy(a => a.AccountEmail),
                "role" => query.IsDescending
                    ? accountsQuery.OrderByDescending(a => a.AccountRole)
                    : accountsQuery.OrderBy(a => a.AccountRole),
                _ => accountsQuery.OrderBy(a => a.AccountID)
            };
        }
        else
        {
            accountsQuery = accountsQuery.OrderBy(a => a.AccountID);
        }

        // Get total count
        var totalCount = await accountsQuery.CountAsync();

        // Apply pagination
        var accounts = await accountsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Convert to business models
        var accountModels = accounts.Select(a => new AccountModel
        {
            AccountID = a.AccountID,
            AccountName = a.AccountName,
            AccountEmail = a.AccountEmail,
            AccountRole = a.AccountRole
        }).ToList();

        _logger.LogInformation($"Retrieved {accountModels.Count} accounts out of {totalCount} total");

        return new Pagination<AccountModel>(accountModels, totalCount, query.Page, query.PageSize);
    }

    /// <summary>
    /// Create new account
    /// </summary>
    public async Task<AccountModel> CreateAccountAsync(CreateAccountModel model)
    {
        _logger.LogInformation($"Creating account: {model.AccountEmail}");

        // Check if email already exists
        if (await _unitOfWork.AccountRepository.FirstOrDefaultAsync(a => a.AccountEmail == model.AccountEmail) != null)
        {
            _logger.LogWarning($"Email {model.AccountEmail} already exists");
            throw ErrorHelper.Conflict("Email already exists.");
        }

        // Hash password
        var hashedPassword = new PasswordHasher<SystemAccount>().HashPassword(null, model.Password);

        var account = new SystemAccount
        {
            AccountName = model.AccountName,
            AccountEmail = model.AccountEmail,
            AccountPassword = hashedPassword,
            AccountRole = model.AccountRole
        };

        await _unitOfWork.AccountRepository.CreateAsync(account);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Account created successfully with ID: {account.AccountID}");

        return new AccountModel
        {
            AccountID = account.AccountID,
            AccountName = account.AccountName,
            AccountEmail = account.AccountEmail,
            AccountRole = account.AccountRole
        };
    }

    /// <summary>
    /// Update existing account
    /// </summary>
    public async Task<AccountModel> UpdateAccountAsync(int id, UpdateAccountModel model)
    {
        _logger.LogInformation($"Updating account with ID: {id}");

        var account = await _unitOfWork.AccountRepository.GetByIdAsync(id);

        if (account == null)
        {
            _logger.LogWarning($"Account with ID {id} not found");
            throw ErrorHelper.NotFound($"Account with ID {id} not found.");
        }

        // Check if email is being changed and already exists
        if (model.AccountEmail != account.AccountEmail)
        {
            var existingAccount = await _unitOfWork.AccountRepository.FirstOrDefaultAsync(
                a => a.AccountEmail == model.AccountEmail && a.AccountID != id);

            if (existingAccount != null)
            {
                _logger.LogWarning($"Email {model.AccountEmail} already exists");
                throw ErrorHelper.Conflict("Email already exists.");
            }
        }

        account.AccountName = model.AccountName;
        account.AccountEmail = model.AccountEmail;
        account.AccountRole = model.AccountRole;

        await _unitOfWork.AccountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Account with ID {id} updated successfully");

        return new AccountModel
        {
            AccountID = account.AccountID,
            AccountName = account.AccountName,
            AccountEmail = account.AccountEmail,
            AccountRole = account.AccountRole
        };
    }

    /// <summary>
    /// Delete account
    /// </summary>
    public async Task<bool> DeleteAccountAsync(int id)
    {
        _logger.LogInformation($"Deleting account with ID: {id}");

        var account = await _unitOfWork.AccountRepository.GetByIdAsync(id);

        if (account == null)
        {
            _logger.LogWarning($"Account with ID {id} not found");
            throw ErrorHelper.NotFound($"Account with ID {id} not found.");
        }

        await _unitOfWork.AccountRepository.RemoveAsync(account);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Account with ID {id} deleted successfully");

        return true;
    }
}
