using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class ClaimsService : IClaimsService
{
    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        // Lấy ClaimsIdentity
        var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

        var extractedId = AuthenTools.GetCurrentUserId(identity);
        if (int.TryParse(extractedId, out var parsedId))
            GetCurrentUserId = parsedId;
        else
            GetCurrentUserId = 0;

        IpAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public int GetCurrentUserId { get; }

    public string? IpAddress { get; }
}


