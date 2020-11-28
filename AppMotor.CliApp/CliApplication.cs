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
using System.Diagnostics;

using AppMotor.CliApp.Properties;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Logging;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp
{
    /// <summary>
    /// Base class for .NET console applications. Use <see cref="Start{TApp}"/> as entry point.
    /// </summary>
    public abstract class ConsoleApplication
    {
        private static bool s_tlsSettingsApplied;

        /// <summary>
        /// The exit code to use when an unhandled exception led to the termination
        /// of the process.
        /// </summary>
        [PublicAPI]
        protected virtual int ExitCodeOnException => -1;

        /// <summary>
        /// Whether to display a "Press any key to exit..." message when the process
        /// terminates.
        /// </summary>
        [PublicAPI]
        protected virtual bool WaitForKeyPressOnExit => false;

        /// <summary>
        /// Starts the specified application.
        /// </summary>
        /// <returns>The exit code to use.</returns>
        [PublicAPI, MustUseReturnValue]
        public static int Start<TApp>(string[] args) where TApp : ConsoleApplication, new()
        {
            if (!s_tlsSettingsApplied)
            {
                TlsSettings.ApplyToCurrentProcess();
                s_tlsSettingsApplied = true;
            }

            var app = new TApp();

            int exitCode;

            try
            {
                exitCode = app.Run(args);
            }
            catch (Exception ex) when (!Debugger.IsAttached)
            {
                app.OnUnhandledException(ex);
                exitCode = app.ExitCodeOnException;
            }

            if ((Debugger.IsAttached || app.WaitForKeyPressOnExit) && !Terminal.IsInputRedirected)
            {
                Terminal.WriteLine();
                Terminal.WriteLine(LocalizableResources.PressAnyKeyToExit);
                Terminal.ReadKey();
            }

            return exitCode;
        }

        /// <summary>
        /// Called for any unhandled exception that is thrown by <see cref="Run"/>.
        /// </summary>
        protected virtual void OnUnhandledException(Exception exception)
        {
            PrintUnhandledException(exception, supportMessage: null);
        }

        /// <summary>
        /// Writes the specified exception to the Console.
        /// </summary>
        /// <param name="exception">The exception to print</param>
        /// <param name="supportMessage">An optional message with information about what to do
        /// with the exception (e.g. link to bug tracker).</param>
        [PublicAPI]
        public static void PrintUnhandledException(Exception exception, string? supportMessage)
        {
            bool printSupportMessage = PrintUnhandledException(exception);

            if (supportMessage != null && printSupportMessage)
            {
                Terminal.WriteLine();
                Terminal.WriteLine((TextInMagenta)supportMessage);
                Terminal.WriteLine();
            }
        }

        [MustUseReturnValue]
        private static bool PrintUnhandledException(Exception exception)
        {
            bool printSupportMessage;

            if (exception is AggregateException aggregateException)
            {
                printSupportMessage = false;
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    if (PrintUnhandledException(innerException))
                    {
                        printSupportMessage = true;
                    }
                }
            }
            else if (exception is ErrorMessageException)
            {
                Terminal.WriteLine((TextInRed)exception.Message);
                printSupportMessage = false;
            }
            else
            {
                Terminal.WriteLine((TextInRed)exception.ToStringExtended());
                printSupportMessage = true;
            }

            return printSupportMessage;
        }

        /// <summary>
        /// Executes the actual program.
        /// </summary>
        /// <param name="args">The args passed from the command line.</param>
        /// <returns>The exit code to return to the operating system.</returns>
        [PublicAPI, MustUseReturnValue]
        protected abstract int Run(string[] args);
    }
}
