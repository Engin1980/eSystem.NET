using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Math.Matrices.Exceptions
{
    public class InvalidDimensionException : EMatrixException
    {
        public InvalidDimensionException(string message) : base(message) { }
        public InvalidDimensionException(Matrix matrix, int expectedRowCount, int expectedColumnCount) :
            base("Invalid dimension of matrix - found " + matrix.GetDimensionsToString("x") + 
            ", requested " + expectedRowCount + "x" + expectedColumnCount) { }
        public InvalidDimensionException (Matrix matrixA, Matrix matrixB) :
            base ("Invalid dimension of matrices - " + matrixA.GetDimensionsToString("x") + " vs. " + matrixB.GetDimensionsToString("x")) { }
        public InvalidDimensionException(int invalidValue, int requestedMinimum, int requestedMaximum) :
          base("Invalid dimension value - entered: " + invalidValue + "; requested from " + requestedMinimum + " to " + requestedMaximum) { }
    }
}
