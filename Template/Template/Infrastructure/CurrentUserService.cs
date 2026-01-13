using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Template.Infrastructure
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            _HttpContextAccessor = accessor;
        }

        public Guid UserId
        {
            get
            {
                var user = _HttpContextAccessor.HttpContext?.User;

                if (user == null || !user.Identity.IsAuthenticated)
                    return Guid.Empty;

                var claim = user.FindFirst(ClaimTypes.NameIdentifier);

                return Guid.TryParse(claim?.Value, out var id) ? id : Guid.Empty;
            }
        }

        public string Username =>
            _HttpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public bool IsAuthenticated =>
            _HttpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
