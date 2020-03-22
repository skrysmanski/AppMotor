/*
Copyright 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
        [NotNull]
        internal const string DEFAULT_MESSAGE = "The specified value is invalid.";

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
            : base(message ?? DEFAULT_MESSAGE, innerException)
        {
            this.ValueName = valueName;
        }
    }
}
