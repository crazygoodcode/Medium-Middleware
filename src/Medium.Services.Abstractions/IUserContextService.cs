using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Medium.Services.Abstractions
{
    public interface IUserContextService
    {
        string GetUserName();
        Task<string> GetUserNameAsync(CancellationToken cancellationToken = default);
    }
}
