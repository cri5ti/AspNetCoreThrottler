using System;

namespace ChristianMihai.AspNetCoreThrottler
{
  class RateLimitEntry
  {
    // How many requests have we had so far
    public int RequestCount { get; private set; }
    
    // When did we start tracking this partition
    public DateTime CreatedAt { get; private set; }
   
    // Based on all the requests we had so far
    // when is this partition going to be idle?
    public DateTime CurrentLevel { get; private set; }
    
    // When can we completely release this partition?
    public DateTime ExpiresAt { get; private set; }

    
    public RateLimitEntry(TimeSpan expire)
    {
      Reset(expire);
    }

    internal void Reset(TimeSpan expire)
    {
      RequestCount = 0;
      CreatedAt = DateTime.Now;
      CurrentLevel = CreatedAt;
      ExpiresAt = CreatedAt + expire;
    }

    internal void Next(TimeSpan duration)
    {
      lock (this)
      {
        RequestCount++;
        CurrentLevel += duration;
        ExpiresAt += duration;
      }
    }
  }
}