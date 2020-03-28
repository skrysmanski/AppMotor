#region License
// Copyright 2020 - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

using AppWeave.Core.Exceptions;
using AppWeave.Core.Utils;

using JetBrains.Annotations;

namespace AppWeave.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Exception"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// The same as <see cref="Exception.Data"/> but makes it safer to use. For details,
        /// <see cref="ExceptionDataAccessor"/>.
        /// </summary>
        [PublicAPI]
        public static ExceptionDataAccessor GetData([NotNull] this Exception exception)
        {
            return new ExceptionDataAccessor(exception);
        }

        /// <summary>
        /// Allows you to add exception data to this exception. This is basically the
        /// same as <c>exception.Data[key] = value;</c> with some added safety measures
        /// to prevent exceptions (like using <see cref="ExceptionDataAccessor"/>).
        /// </summary>
        /// <param name="exception">The exception to add data to.</param>
        /// <param name="key">The key. Can be anything but strings are recommended.</param>
        /// <param name="value">The value.</param>
        [PublicAPI]
        public static void AddData([NotNull] this Exception exception, [NotNull] object key, [CanBeNull] object value)
        {
            Verify.ParamNotNull(exception, nameof(exception));
            Verify.ParamNotNull(key, nameof(key));

            var exceptionData = exception.GetData();

            if (exceptionData.IsReadOnly)
            {
                return;
            }

            // NOTE: With .NET Standard 2.1 we no longer need to check if the key and value are
            //   serializable as this requirement has apparently been dropped. It still exists
            //   in the .NET Framework but since .NET Standard 2.1 is not compatible with the
            //   .NET Framework, we don't need to care.
            exceptionData[key] = value;
        }
    }
}
