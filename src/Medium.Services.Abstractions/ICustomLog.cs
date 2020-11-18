using System.Threading;
using System.Threading.Tasks;

namespace Medium.Services.Abstractions
{
    public interface ICustomLog 
    {
        void Log<T>(T payload);
        Task LogAsync<T>(T payload, CancellationToken cancellationToken = default);
    }
}
