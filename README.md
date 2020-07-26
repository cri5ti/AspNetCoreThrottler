A lightweight ASP.NET Core Throttling.

[![NuGet](https://buildstats.info/nuget/AspNetCoreThrottler)](https://www.nuget.org/packages/AspNetCoreThrottler/)

## Usage

```csharp
public void ConfigureServices(IServiceCollection services) {
  //...
  services.Configure<RateLimitOptions>(config =>
  {
    config.RequestRateMs = 250;
    config.LimitSoft = 4;
    config.LimitHard = 50;
  });
  //...
}
```

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  // ...
  app.UseThrottling();
  // ...
}
```


## Read more about it:
[https://christianmihai.com/lightweight-aspnet-core-throttling/](https://www.christianmihai.com/lightweight-aspnet-core-throttling/)


## Roadmap:

This is not a production ready NuGet yet, but feel free to use it as a basis for your project. 
- [x] throttling
- [x] blocking 
- [x] partitions
- [ ] extract partitioning  
- [ ] cleanup expired partitions
