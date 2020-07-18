#region License

// Copyright  - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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

using JetBrains.Annotations;

using Shouldly;

namespace AppWeave.Core.TestUtils
{
    internal sealed class ExceptionCreator<TException> where TException : Exception
    {
        [NotNull]
        private readonly string m_exceptionMessage;

        [CanBeNull]
        private readonly Exception m_innerException;

        private ExceptionCreator([NotNull] string exceptionMessage, [CanBeNull] Exception innerException)
        {
            this.m_exceptionMessage = exceptionMessage;
            this.m_innerException = innerException;
        }

        public static void CreateAndThrow(
                [NotNull] string exceptionMessage = "Some error text",
                [CanBeNull] Exception innerException = null,
                int stackDepth = 5
            )
        {
            var creator = new ExceptionCreator<TException>(exceptionMessage, innerException);
            creator.MethodOnTheStacktraceA(stackDepth - 1);
        }

        [NotNull]
        public static TException CreateAndCatch(
                [NotNull] string exceptionMessage = "Some error text",
                [CanBeNull] Exception innerException = null,
                int stackDepth = 5
            )
        {
            try
            {
                CreateAndThrow(exceptionMessage, innerException, stackDepth);
            }
            catch (TException ex)
            {
                return ex;
            }

            throw new UnexpectedBehaviorException("We should never get here.");
        }

        private void MethodOnTheStacktraceA(int remainingStackFrames)
        {
            if (remainingStackFrames <= 0)
            {
                throw CreateException();
            }
            else
            {
                MethodOnTheStacktraceB(remainingStackFrames - 1);
            }
        }

        private void MethodOnTheStacktraceB(int remainingStackFrames)
        {
            if (remainingStackFrames <= 0)
            {
                throw CreateException();
            }
            else
            {
                MethodOnTheStacktraceC(remainingStackFrames - 1);
            }
        }

        private void MethodOnTheStacktraceC(int remainingStackFrames)
        {
            if (remainingStackFrames <= 0)
            {
                throw CreateException();
            }
            else
            {
                MethodOnTheStacktraceD(remainingStackFrames - 1);
            }
        }

        private void MethodOnTheStacktraceD(int remainingStackFrames)
        {
            if (remainingStackFrames <= 0)
            {
                throw CreateException();
            }
            else
            {
                MethodOnTheStacktraceE(remainingStackFrames - 1);
            }
        }

        private void MethodOnTheStacktraceE(int remainingStackFrames)
        {
            if (remainingStackFrames <= 0)
            {
                throw CreateException();
            }
            else
            {
                MethodOnTheStacktraceF(remainingStackFrames - 1);
            }
        }

        private void MethodOnTheStacktraceF(int remainingStackFrames)
        {
            if (remainingStackFrames <= 0)
            {
                throw CreateException();
            }
            else
            {
                MethodOnTheStacktraceA(remainingStackFrames - 1);
            }
        }

        [NotNull]
        private TException CreateException()
        {

            TException newException;

            if (this.m_innerException != null)
            {
                newException = (TException)Activator.CreateInstance(typeof(TException), this.m_exceptionMessage, this.m_innerException);
            }
            else
            {
                newException = (TException)Activator.CreateInstance(typeof(TException), this.m_exceptionMessage);
            }

            newException.ShouldNotBeNull();

            return newException;
        }
    }
}
