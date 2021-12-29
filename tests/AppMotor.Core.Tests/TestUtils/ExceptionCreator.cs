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

using AppMotor.Core.Exceptions;

using Shouldly;

namespace AppMotor.Core.TestUtils;

internal sealed class ExceptionCreator<TException> where TException : Exception
{
    private readonly string _exceptionMessage;

    private readonly Exception? _innerException;

    private ExceptionCreator(string exceptionMessage, Exception? innerException)
    {
        this._exceptionMessage = exceptionMessage;
        this._innerException = innerException;
    }

    public static void CreateAndThrow(
            string exceptionMessage = "Some error text",
            Exception? innerException = null,
            int stackDepth = 5
        )
    {
        var creator = new ExceptionCreator<TException>(exceptionMessage, innerException);
        creator.MethodOnTheStacktraceA(stackDepth - 1);
    }

    public static TException CreateAndCatch(
            string exceptionMessage = "Some error text",
            Exception? innerException = null,
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

    private TException CreateException()
    {

        TException newException;

        if (this._innerException != null)
        {
            newException = (TException)Activator.CreateInstance(typeof(TException), this._exceptionMessage, this._innerException)!;
        }
        else
        {
            newException = (TException)Activator.CreateInstance(typeof(TException), this._exceptionMessage)!;
        }

        newException.ShouldNotBeNull();

        return newException;
    }
}