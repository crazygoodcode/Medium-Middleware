using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Medium.Services.Abstractions;
using System;
using System.IO;

namespace Medium.Web.Services
{
    public sealed class FileSystemLogService : IFileSystemLogService
    {
        private const string DefaultDirectory = "C:\\Temp";

        public FileSystemLogService() { }

        public void Log<T>(T payload)
        {
            LogAsync(payload).Wait();
        }

        public async Task LogAsync<T>(T payload, CancellationToken cancellationToken = default)
        {
            if (Directory.Exists(DefaultDirectory))
            {
                await File.WriteAllBytesAsync($"{DefaultDirectory}\\{Guid.NewGuid().ToString("N")}.json", 
                    JsonSerializer.SerializeToUtf8Bytes(payload), 
                    cancellationToken);
            }
        }
    }
}
