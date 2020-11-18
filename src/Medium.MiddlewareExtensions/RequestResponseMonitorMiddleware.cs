using Medium.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Medium.MiddlewareExtensions
{
    /// <summary>
    /// Simple request/response monitoring. For production systems use a high resolution timer.
    /// </summary>
    public sealed class RequestResponseMonitorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFileSystemLogService _fileSystemLogService;
        public RequestResponseMonitorMiddleware(RequestDelegate next,
            IFileSystemLogService fileSystemLogService)
        {
            _next = next;
            _fileSystemLogService = fileSystemLogService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var request = context.Request;
                var stopWatch = Stopwatch.StartNew();
                var requestBodyDetails = await ReadRequestBody(request);
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    var response = context.Response;

                    response.Body = responseBody;

                    await _next(context);

                    stopWatch.Stop();

                    var requestResponseBody = await ReadResponseBody(response);

                    await responseBody.CopyToAsync(originalBodyStream);

                    await _fileSystemLogService.LogAsync(new
                    {
                        requestBodyDetails.url,
                        requestBodyDetails.body,
                        responseBody = requestResponseBody,
                        totalMilliseconds = stopWatch.Elapsed.TotalMilliseconds
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                await _next(context);
            }
        }

        private async Task<(string url, string body)> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            var bodyAsText = Encoding.UTF8.GetString(buffer);

            request.Body.Seek(0, SeekOrigin.Begin);

            var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            return (url, bodyAsText);
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();

            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }
    }
}
