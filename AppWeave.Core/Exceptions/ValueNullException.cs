using System;

using JetBrains.Annotations;

namespace AppWeave.Core.Exceptions
{
    /// <summary>
    /// Equivalent to <see cref="ArgumentNullException"/> - just for values that are not arguments/parameters.
    /// </summary>
    [PublicAPI]
    public class ValueNullException : ValueException
    {
        public ValueNullException()
            : this(message: null, valueName: null, innerException: null)
        {
        }

        public ValueNullException([CanBeNull] string message)
            : this(message: message, valueName: null, innerException: null)
        {
        }

        public ValueNullException([CanBeNull] string message, [CanBeNull] string valueName)
            : this(message: message, valueName: valueName, innerException: null)
        {
        }

        public ValueNullException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : this(message: message, valueName: null, innerException: innerException)
        {
        }

        public ValueNullException([CanBeNull] string message, [CanBeNull] string valueName, [CanBeNull] Exception innerException)
            : base(message ?? "The specified value is null.", valueName, innerException)
        {
        }
    }
}
