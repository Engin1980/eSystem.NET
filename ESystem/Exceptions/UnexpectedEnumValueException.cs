using ESystem.Asserting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Exceptions
{
    /// <summary>
    /// Supposed to be invoked in switch/cases, when unexpected not-null value occurs.
    /// </summary>
    public class UnexpectedEnumValueException : Exception
    {
        public object EnumValue { get; private set; }
        public Type EnumType { get; private set; }
        public UnexpectedEnumValueException(Enum unexpectedEnumValue) : base($"Unexpected enum value '{unexpectedEnumValue}' for type '{unexpectedEnumValue.GetType().FullName}'.")
        {
            EAssert.Argument.IsNotNull(unexpectedEnumValue, nameof(unexpectedEnumValue));
            this.EnumValue = unexpectedEnumValue;
            this.EnumType = unexpectedEnumValue.GetType();
        }
    }
}
