using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ESystem.ThreadTracking;

public class TrackedTimer : IDisposable
{
  private readonly static List<TrackedTimer> inner = new();

  private System.Timers.Timer Timer { get; init; }
  public Thread? LaunchingThread { get; private set; }
  public Thread TrackingThread { get; private set; }
  public string Name { get; private init; }
  public bool IsEnabled => Timer.Enabled;
  public bool AutoReset
  {
    get => Timer.AutoReset;
    set => Timer.AutoReset = value;
  }
  public double Interval
  {
    get => Timer.Interval;
    set => Timer.Interval = value;
  }
  public event ElapsedEventHandler? Elapsed
  {
    add => Timer.Elapsed += value;
    remove => Timer.Elapsed -= value;
  }
  public void Close()
  {
    Timer.Close();
  }

  public static TrackedTimer Create(int? interval = null, string? name = null)
  {
    System.Timers.Timer timer = interval is null ? new() : new(interval.Value);
    TrackedTimer tt = new(timer, name ?? $"TrackedTimer {timer.GetHashCode()}");
    inner.Add(tt);
    return tt;
  }
  public static TrackedTimer Create(double? interval = null, string? name = null)
  {
    System.Timers.Timer timer = interval is null ? new() : new(interval.Value);
    TrackedTimer tt = new(timer, name ?? $"TrackedTimer {timer.GetHashCode()}");
    inner.Add(tt);
    return tt;
  }
  public static TrackedTimer Create(TimeSpan? interval = null, string? name = null)
  {
    System.Timers.Timer timer = interval is null ? new() : new(interval.Value.TotalMilliseconds);
    TrackedTimer tt = new(timer, name ?? $"TrackedTimer {timer.GetHashCode()}");
    inner.Add(tt);
    return tt;
  }

  public static TrackedTimer Track(System.Timers.Timer timer, string? name = null)
  {
    if (timer == null) throw new ArgumentNullException(nameof(timer));
    TrackedTimer tt = new(timer, name ?? $"TrackedTimer {timer.GetHashCode()}");
    inner.Add(tt);
    return tt;
  }

  public TrackedTimer(System.Timers.Timer timer, string name)
  {
    Timer = timer;
    this.LaunchingThread = null;
    this.TrackingThread = Thread.CurrentThread;
    Name = name;
  }

  public override string ToString() => $"TrackedTimer: {Name}";

  public void Start()
  {
    if (Timer.Enabled) return;
    this.LaunchingThread = Thread.CurrentThread;
    Timer.Start();
  }

  public void Stop()
  {
    if (!Timer.Enabled) return;
    Timer.Stop();
    this.LaunchingThread = null;
  }

  public void Dispose()
  {
    this.Timer.Dispose();
    inner.Remove(this);
  }

  public static List<TrackedTimer> GetAll() => inner.ToList();
  public static List<TrackedTimer> GetByTrackingThread(Thread trackingThread) => inner.Where(q => q.TrackingThread == trackingThread).ToList();
  public static List<TrackedTimer> GetByLaunchingOrTrackingThread(Thread thread) => inner.Where(q => q.LaunchingThread == thread || q.TrackingThread == thread).ToList();
  public static List<TrackedTimer> GetByLaunchingThread(Thread launchingThread) => inner.Where(q => q.LaunchingThread == launchingThread).ToList();
}
