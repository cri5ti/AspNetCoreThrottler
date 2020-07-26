using ChristianMihai.AspNetCoreThrottler;
using Microsoft.AspNetCore.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
  public static class RateLimitingExtensions
  {
    public static IApplicationBuilder UseThrottling(this IApplicationBuilder app)
    {
      return app.UseMiddleware<RateThrottlerMiddleware>();
    }
  }
}