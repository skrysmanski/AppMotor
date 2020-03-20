using System;

using JetBrains.Annotations;

namespace AppWeave.Core.Exceptions
{
    /// <summary>
    /// Equivalent to <see cref="ArgumentOutOfRangeException"/> - just for values that are not arguments/parameters.
    /// </summary>
    [PublicAPI]
    public class ValueOutOfRangeException : ValueException
    {
        public ValueOutOfRangeException()
            : this(message: null, valueName: null, innerException: null)
        {
        }

        public ValueOutOfRangeException([CanBeNull] string message)
            : this(message: message, valueName: null, innerException: null)
        {
        }

        public ValueOutOfRangeException([CanBeNull] string message, [CanBeNull] string valueName)
            : this(message: message, valueName: valueName, innerException: null)
        {
        }

        public ValueOutOfRangeException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : this(message: message, valueName: null, innerException: innerException)
        {
        }

        public ValueOutOfRangeException([CanBeNull] string message, [CanBeNull] string valueName, [CanBeNull] Exception innerException)
            : base(message ?? "The specified value is outside of the range of valid values.", valueName, innerException)
        {
        }
    }
}
