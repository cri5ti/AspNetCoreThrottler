namespace ChristianMihai.AspNetCoreThrottler
{
  public class RateLimitOptions
  {
    public int RequestRateMs { get; set; }
    public int LimitSoft { get; set; }
    public int LimitHard { get; set; }
    public string HardLimitMessage { get; set; }
  }
}