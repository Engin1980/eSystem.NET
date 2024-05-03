using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Exceptions
{
    /// <summary>
    /// Supposed to be invoked on code, which should be implemented in near future.
    /// </summary>
    public class ToDoException : Exception
    {
        public ToDoException()
        {
        }

        public ToDoException(string? message) : base(message)
        {
        }
    }
}
