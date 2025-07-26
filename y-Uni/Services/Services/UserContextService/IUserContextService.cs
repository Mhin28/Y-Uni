using System;

namespace Services.Services.UserContextService
{
    public interface IUserContextService
    {
        Guid GetCurrentUserId();
        string GetCurrentUserName();
        string GetCurrentUserRole();
        bool IsAuthenticated();
    }
}