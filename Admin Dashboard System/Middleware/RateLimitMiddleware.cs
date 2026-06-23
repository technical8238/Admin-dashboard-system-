using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace Admin_Dashboard_System.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, DateTime> _requestLog = new();
        private static readonly int _maxRequestsPerMinute = 100;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var path = context.Request.Path.Value;

            // Skip rate limiting for static files and SignalR
            if (path != null && (path.StartsWith("/lib") || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/images") || path.StartsWith("/dashboardHub")))
            {
                await _next(context);
                return;
            }

            if (ipAddress != null)
            {
                var key = $"{ipAddress}:{DateTime.UtcNow.Minute}";
                var requestCount = _requestLog.Count(x => x.Key.StartsWith(ipAddress) && x.Value > DateTime.UtcNow.AddMinutes(-1));

                if (requestCount >= _maxRequestsPerMinute)
                {
                    _logger.LogWarning($"Rate limit exceeded for IP: {ipAddress}");
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                    return;
                }

                _requestLog[key] = DateTime.UtcNow;

                // Clean up old entries
                var now = DateTime.UtcNow;
                foreach (var entry in _requestLog.Where(x => x.Value < now.AddMinutes(-1)).ToList())
                {
                    _requestLog.TryRemove(entry.Key, out _);
                }
            }

            await _next(context);
        }
    }
}
