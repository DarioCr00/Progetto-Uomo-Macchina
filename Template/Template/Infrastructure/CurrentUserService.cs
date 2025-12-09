using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Template.Infrastructure;

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
                var value = _HttpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public string Username =>
            _HttpContextAccessor.HttpContext?.Items["Username"]?.ToString();

        public bool IsAuthenticated =>
            UserId != Guid.Empty;
    }
}
