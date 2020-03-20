using System;

using JetBrains.Annotations;

namespace AppWeave.Core.Exceptions
{
    /// <summary>
    /// Equivalent to <see cref="ArgumentException"/> - just for values that are not arguments/parameters.
    /// </summary>
    [PublicAPI]
    public class ValueException : Exception
    {
        [CanBeNull]
        public string ValueName { get; }

        [NotNull]
        public override string Message
        {
            get
            {
                var message = base.Message;

                if (string.IsNullOrEmpty(this.ValueName))
                {
                    return message;
                }

                return message + Environment.NewLine + "Value name: " + this.ValueName;
            }
        }

        [PublicAPI]
        public ValueException()
            : this(message: null, valueName: null, innerException: null)
        {
        }

        [PublicAPI]
        public ValueException([CanBeNull] string message)
            : this(message: message, valueName: null, innerException: null)
        {
        }

        [PublicAPI]
        public ValueException([CanBeNull] string message, [CanBeNull] string valueName)
            : this(message: message, valueName: valueName, innerException: null)
        {
        }

        [PublicAPI]
        public ValueException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : this(message: message, valueName: null, innerException: innerException)
        {
        }

        [PublicAPI]
        public ValueException([CanBeNull] string message, [CanBeNull] string valueName, [CanBeNull] Exception innerException)
            : base(message ?? "The specified value is invalid.", innerException)
        {
            this.ValueName = valueName;
        }
    }
}
