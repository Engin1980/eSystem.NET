using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Math.Matrices
{
    public class EMatrixException : Exception
    {
        public EMatrixException(string message) : base(message) { }
    }
}
