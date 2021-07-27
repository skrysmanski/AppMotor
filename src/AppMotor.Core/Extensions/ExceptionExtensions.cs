#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions
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
        public static ExceptionDataAccessor GetData(this Exception exception)
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
        public static void AddData(this Exception exception, object key, object? value)
        {
            Validate.ArgumentWithName(nameof(exception)).IsNotNull(exception);
            Validate.ArgumentWithName(nameof(key)).IsNotNull(key);

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

        /// <summary>
        /// Captures this exception so that it can be rethrown later without losing its
        /// stacktrace. For more details, see <see cref="Rethrow"/>.
        /// </summary>
        [PublicAPI, Pure]
        public static ExceptionDispatchInfo Capture(this Exception exception)
        {
            return ExceptionDispatchInfo.Capture(exception);
        }

        /// <summary>
        /// Rethrows the specified exception without losing its original stacktrace. However,
        /// the stacktrace will still be modified. It'll get the stacktrace of how the control
        /// flow got to this method plus the following string inserted in the stack trace:
        /// "End of stack trace from the previous location where the exception was thrown".
        ///
        /// <para>This method is similar to <c>throw;</c> but also works on inner exceptions
        /// stored in the caught exception (where <c>throw;</c> would not work). So the rule
        /// of thumb is: Use <c>throw;</c> wherever possible and use this method where it's
        /// not possible.</para>
        ///
        /// <para>You should use this method like this: <c>throw exception.Rethrow();</c></para>
        /// </summary>
        [PublicAPI, ContractAnnotation("=>halt")]
        // NOTE: The "Exception" return type here is necessary so that this call can be used
        //   in a "throw" statement - see documentation.
        public static Exception Rethrow(this Exception exception)
        {
            exception.Capture().Throw();
            throw new UnexpectedBehaviorException("We should never get here.");
        }

        /// <summary>
        /// "Unrolls" this <see cref="AggregateException"/>, if possible. "Unrolling" here means
        /// that if the <see cref="AggregateException"/> only contains 1 inner exception, this inner
        /// exception will be thrown instead.
        ///
        /// <para>You should use this method like this: <c>throw exception.UnrollIfPossible(...);</c></para>
        ///
        /// <para>This method is primarily intended in conjunction with any of the <c>Wait</c> methods
        /// in <see cref="Task"/> or with <see cref="Task{TResult}.Result"/> - as they all throw
        /// <see cref="AggregateException"/>s no matter how many exceptions actually occurred.</para>
        /// </summary>
        /// <param name="aggregateException">The exception to unroll</param>
        /// <param name="deepUnroll">If <c>true</c> and the only inner exception is also a
        /// <seealso cref="AggregateException"/>, this inner exception will also be unrolled (and so forth).
        /// If <c>false</c>, only this exception will be unrolled. In case of doubt, use <c>false</c> and
        /// change it to <c>true</c> if see the need.</param>
        /// <param name="preventUnrollingOnExistingExceptionData">When unrolling, should existing exception
        /// data (<see cref="Exception.Data"/>) prevent unrolling (<c>true</c>; the default). The reasoning
        /// here is that if the <see cref="AggregateException"/> itself contains exception data, it would
        /// be lost on unrolling.</param>
        [PublicAPI, ContractAnnotation("=>halt")]
        public static Exception UnrollIfPossible(
                this AggregateException aggregateException,
                bool deepUnroll = false,
                bool preventUnrollingOnExistingExceptionData = true
            )
        {
            Validate.ArgumentWithName(nameof(aggregateException)).IsNotNull(aggregateException);

            Exception exceptionToThrow = GetUnrolledException(aggregateException, deepUnroll, preventUnrollingOnExistingExceptionData);

            throw exceptionToThrow.Rethrow();
        }

        private static Exception GetUnrolledException(
                AggregateException aggregateException,
                bool deepUnroll,
                bool preventUnrollingOnExistingExceptionData
            )
        {
            if (preventUnrollingOnExistingExceptionData && aggregateException.GetData().Count != 0)
            {
                return aggregateException;
            }

            if (aggregateException.InnerExceptions.Count != 1)
            {
                return aggregateException;
            }

            var onlyInnerException = aggregateException.InnerExceptions[0];

            if (deepUnroll && onlyInnerException is AggregateException innerAggregateException)
            {
                return GetUnrolledException(
                    innerAggregateException,
                    deepUnroll: true,
                    preventUnrollingOnExistingExceptionData: preventUnrollingOnExistingExceptionData
                );
            }
            else
            {
                return onlyInnerException;
            }
        }
    }
}
