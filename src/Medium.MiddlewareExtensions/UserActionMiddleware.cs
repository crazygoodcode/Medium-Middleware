using Medium.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Medium.MiddlewareExtensions
{
    public sealed class UserActionMiddleware : IMiddleware
    {
        private readonly IFileSystemLogService _fileSystemLogService;
        private readonly IUserContextService _userContext;

        public UserActionMiddleware(IFileSystemLogService fileSystemLogService,
            IUserContextService userContext)
        {
            _fileSystemLogService = fileSystemLogService;
            _userContext = userContext;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";

            await _fileSystemLogService.LogAsync(new
            {
                url,
                user = await _userContext.GetUserNameAsync()
            }).ConfigureAwait(false);

            await next(context);
        }
    }
}
