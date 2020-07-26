using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChristianMihai.AspNetCoreThrottler
{
  internal class RateThrottlerMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IOptions<RateLimitOptions> _options;

    private readonly ConcurrentDictionary<string, RateLimitEntry> _partitions;
    private readonly ILogger<RateThrottlerMiddleware> _logger;

    public RateThrottlerMiddleware(
      RequestDelegate next,
      IOptions<RateLimitOptions> options,
      ILogger<RateThrottlerMiddleware> logger
    )
    {
      _next = next;
      _options = options;
      _logger = logger;
      _partitions = new ConcurrentDictionary<string, RateLimitEntry>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var opts = _options.Value;

      // is throttling disabled?
      if (opts.RequestRateMs == 0 || (opts.LimitHard == 0 && opts.LimitSoft == 0))
      {
        await _next(context);
        return;
      }

      var now = DateTime.Now;
      var key = context.Request.Headers["Domain"].FirstOrDefault() ?? "default";

      var requestDuration = TimeSpan.FromMilliseconds(opts.RequestRateMs);
      var maxLimit = requestDuration * opts.LimitHard;
      var entry = _partitions.GetOrAdd(key, (k, arg) => new RateLimitEntry(arg), maxLimit);

      if (entry.ExpiresAt < now)
      {
        _logger.LogDebug("Expired bucket, resetting.");
        entry.Reset(maxLimit);
      }

      // TODO expired buckets cleanup

      var queueSize = (int)((entry.CurrentLevel - now) / requestDuration);

      _logger.LogTrace("New request, queue size: {queueSize}", queueSize);

      // hard limit
      if (opts.LimitHard > 0 && queueSize >= opts.LimitHard)
      {
        _logger.LogWarning("Request aborted, too many requests: {queueSize} / {hardLimit}", queueSize, opts.LimitHard);
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.Response.WriteAsync(opts.HardLimitMessage ?? "Too many requests");
        return;
      }

      entry.Next(requestDuration);

      // throttling
      if (opts.LimitSoft > 0 && queueSize >= opts.LimitSoft)
      {
        _logger.LogTrace("Throttling request: {queueSize} / {softLimit}", queueSize, opts.LimitSoft);

        var delay = entry.CurrentLevel - now;
        if (delay.Milliseconds > 10)
          await Task.Delay(delay);
      }

      // perform request
      await _next(context);
    }
  }
}