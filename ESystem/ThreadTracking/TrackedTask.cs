using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.ThreadTracking;

public class TrackedTask
{
  private readonly static List<TrackedTask> inner = new();
  private Task Task { get; init; }
  private Thread LaunchingThread { get; init; }
  private string Name { get; init; }

  private TrackedTask(Task task, Thread launchingThread, string name)
  {
    this.Task = task;
    this.LaunchingThread = launchingThread;
    this.Name = name;
  }

  public static Task Run(Task task, string? name = null)
  {
    if (task == null) throw new ArgumentNullException(nameof(task));
    TrackedTask tt = new(task, Thread.CurrentThread, name ?? $"Task {task.Id}");
    inner.Add(tt);
    task.Start();
    return task;
  }

  public static Task Run(Action action, string? name = null)
  {
    if (action == null) throw new ArgumentNullException(nameof(action));
    Task task = new(action);
    return Run(task, name);
  }

  public static Task Track(Task task, string? name = null)
  {
    TrackedTask tt = new(task, Thread.CurrentThread, name ?? $"Task {task.Id}");
    inner.Add(tt);
    return task;
  }

  public static List<TrackedTask> GetAll() => inner.ToList();
  public static List<TrackedTask> Get(Thread launchingThread) => inner.Where(q => q.LaunchingThread == launchingThread).ToList();
}

public class TrackedTask<T>
{
  private readonly static List<TrackedTask<T>> inner = new();
  private Task<T> Task { get; init; }
  private Thread LaunchingThread { get; init; }
  private string Name { get; init; }

  private TrackedTask(Task<T> task, Thread launchingThread, string name)
  {
    this.Task = task;
    this.LaunchingThread = launchingThread;
    this.Name = name;
  }

  public static Task<T> Run(Task<T> task, string? name = null)
  {
    if (task == null) throw new ArgumentNullException(nameof(task));
    TrackedTask<T> tt = new(task, Thread.CurrentThread, name ?? $"Task {task.Id}");
    inner.Add(tt);
    task.Start();
    return task;
  }

  public static Task<T> Run(Func<T> action, string? name = null)
  {
    if (action == null) throw new ArgumentNullException(nameof(action));
    Task<T> task = new(action);
    return Run(task, name);
  }

  public static Task<T> Track(Task<T> task, string? name = null)
  {
    TrackedTask<T> tt = new(task, Thread.CurrentThread, name ?? $"Task {task.Id}");
    inner.Add(tt);
    return task;
  }

  public static List<TrackedTask<T>> GetAll() => inner.ToList();
  public static List<TrackedTask<T>> Get(Thread launchingThread) => inner.Where(q => q.LaunchingThread == launchingThread).ToList();
}

public class Test
{
  public void Main()
  {
    Action a = () => Console.WriteLine("Hello from tracked task!");
    Task.Run(a);
    TrackedTask.Run(a);

    Task.Run(a).Wait();
  }
}
