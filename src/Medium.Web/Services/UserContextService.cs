using Medium.Services.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Medium.Web.Services
{
    public sealed class UserContextService : IUserContextService
    {
        public string GetUserName() => "Medium";

        public Task<string> GetUserNameAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return null;

            return Task.FromResult("Medium");
        }
    }
}
