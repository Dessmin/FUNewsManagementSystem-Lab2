using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.FUNewsManagementSystem.API.Models.Request.Account;
using PRN232.FUNewsManagementSystem.API.Models.Response.Account;
using PRN232.FUNewsManagementSystem.API.Models.Response.Auth;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Account;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.FUNewsManagementSystem.API.Controllers;

/// <summary>
/// Account management endpoints
/// </summary>
[Route("api/accounts")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <returns>Account details</returns>
    /// <response code="200">Returns the account</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Account not found</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get account by ID",
        Description = "Retrieve detailed information about a specific account"
    )]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountById(int id)
    {
        try
        {
            var accountModel = await _accountService.GetAccountByIdAsync(id);

            var response = new AccountResponse
            {
                AccountID = accountModel!.AccountID,
                AccountName = accountModel.AccountName,
                AccountEmail = accountModel.AccountEmail,
                AccountRole = accountModel.AccountRole,
                AccountRoleName = GetRoleName(accountModel.AccountRole)
            };

            return Ok(ApiResult<AccountResponse>.Success(response, "200", "Account retrieved successfully."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<UserResponse>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    /// <summary>
    /// Get paginated list of accounts
    /// </summary>
    /// <param name="request">Query parameters for search, filter, sort, and pagination</param>
    /// <returns>Paginated list of accounts</returns>
    /// <response code="200">Returns the paginated account list</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get list of accounts",
        Description = "Retrieve a paginated list of accounts with support for search, filtering, and sorting"
    )]
    [ProducesResponseType(typeof(ApiResult<AccountListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<AccountListResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccounts([FromQuery] GetAccountsRequest request)
    {
        try
        {
            var queryModel = new AccountQueryModel
            {
                SearchTerm = request.SearchTerm,
                AccountRole = (int?)request.AccountRole,
                IsDescending = request.IsDescending,
                Page = request.Page,
                PageSize = request.PageSize
            };

            var paginatedResult = await _accountService.GetAccountsAsync(queryModel);

            var response = new AccountListResponse
            {
                Items = paginatedResult.Select(a => new AccountResponse
                {
                    AccountID = a.AccountID,
                    AccountName = a.AccountName,
                    AccountEmail = a.AccountEmail,
                    AccountRole = a.AccountRole,
                    AccountRoleName = GetRoleName(a.AccountRole)
                }).ToList(),
                Page = paginatedResult.CurrentPage,
                PageSize = paginatedResult.PageSize,
                TotalItems = paginatedResult.TotalCount,
                TotalPages = paginatedResult.TotalPages
            };

            return Ok(ApiResult<AccountListResponse>.Success(response, "200", "Accounts retrieved successfully."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            return StatusCode(statusCode, ExceptionUtils.CreateErrorResponse<AccountListResponse>(ex));
        }
    }

    /// <summary>
    /// Create new account
    /// </summary>
    /// <param name="request">Account creation data</param>
    /// <returns>Created account</returns>
    /// <response code="201">Account created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="409">Email already exists</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create new account",
        Description = "Create a new account with the provided information"
    )]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            var createModel = new CreateAccountModel
            {
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                Password = request.Password,
                AccountRole = request.AccountRole
            };

            var accountModel = await _accountService.CreateAccountAsync(createModel);

            var response = new AccountResponse
            {
                AccountID = accountModel.AccountID,
                AccountName = accountModel.AccountName,
                AccountEmail = accountModel.AccountEmail,
                AccountRole = accountModel.AccountRole,
                AccountRoleName = GetRoleName(accountModel.AccountRole)
            };

            return CreatedAtAction(
                nameof(GetAccountById),
                new { id = response.AccountID },
                ApiResult<AccountResponse>.Success(response, "201", "Account created successfully.")
            );
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<UserResponse>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    /// <summary>
    /// Update existing account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="request">Updated account data</param>
    /// <returns>Updated account</returns>
    /// <response code="200">Account updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Account not found</response>
    /// <response code="409">Email already exists</response>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update account",
        Description = "Update an existing account's information"
    )]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<AccountResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountRequest request)
    {
        try
        {
            var updateModel = new UpdateAccountModel
            {
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountRole = request.AccountRole
            };

            var accountModel = await _accountService.UpdateAccountAsync(id, updateModel);

            var response = new AccountResponse
            {
                AccountID = accountModel.AccountID,
                AccountName = accountModel.AccountName,
                AccountEmail = accountModel.AccountEmail,
                AccountRole = accountModel.AccountRole,
                AccountRoleName = GetRoleName(accountModel.AccountRole)
            };

            return Ok(ApiResult<AccountResponse>.Success(response, "200", "Account updated successfully."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<UserResponse>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    /// <summary>
    /// Delete account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <returns>Deletion status</returns>
    /// <response code="200">Account deleted successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Account not found</response>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete account",
        Description = "Delete an account from the system"
    )]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        try
        {
            await _accountService.DeleteAccountAsync(id);
            return Ok(ApiResult.Success("200", "Account deleted successfully."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<UserResponse>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    private static string GetRoleName(int role)
    {
        return role switch
        {
            0 => "User",
            1 => "Staff",
            2 => "Lecturer",
            3 => "Student",
            _ => "Unknown"
        };
    }
}
