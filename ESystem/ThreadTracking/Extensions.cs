using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.ThreadTracking
{
  public static class Extensions
  {
    public static Task RunTracked(this Task task, string? name = null) => TrackedTask.Run(task, name);
    public static Task<T> RunTracked<T>(this Task<T> task, string? name = null) => TrackedTask<T>.Run(task, name);

    public static Task Track(this Task task, string? name = null) => TrackedTask.Track(task, name);
    public static Task<T> Track<T>(this Task<T> task, string? name = null) => TrackedTask<T>.Track(task, name);
  }
}
