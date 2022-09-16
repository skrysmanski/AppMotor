// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Extensions;
using AppMotor.Core.Logging;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Logging;

/// <summary>
/// Tests for <see cref="StackTraceCleaner"/>.
/// </summary>
public sealed class StackTraceCleanerTests
{
    public static IEnumerable<object[]> Data
    {
        get
        {
            var testStackTrace = @"
bei System.CommandLine.Help.HelpBuilder..ctor(IConsole console, Int32 maxWidth)
bei System.CommandLine.CommandLineConfiguration.<>c.<.ctor>b__1_0(BindingContext context)
bei System.CommandLine.Invocation.ServiceProvider.<>c__DisplayClass1_0.<.ctor>b__3(IServiceProvider _)
bei System.CommandLine.Invocation.ServiceProvider.GetService(Type serviceType)
bei System.CommandLine.Binding.BindingContext.get_HelpBuilder()
bei System.CommandLine.Invocation.ParseErrorResult.Apply(InvocationContext context)
bei System.CommandLine.Invocation.InvocationPipeline.GetExitCode(InvocationContext context)
bei System.CommandLine.Invocation.InvocationPipeline.InvokeAsync(IConsole console)
bei System.CommandLine.CommandExtensions.InvokeAsync(Command command, String[] args, IConsole console)
bei AppMotor.CliApp.CommandLine.Utils.RootCommandInvoker.InvokeRootCommand(String appDescription, IEnumerable`1 rootSymbols, ICommandHandler commandHandler, ITerminal terminal, String[] args, Func`2 exceptionHandlerFunc) in /_/src/AppMotor.CliApp/CommandLine/Utils/RootCommandInvoker.cs:line 60
bei AppMotor.CliApp.CommandLine.CliApplicationWithCommand.Execute(String[] args, CancellationToken cancellationToken) in /_/src/AppMotor.CliApp/CommandLine/CliApplicationWithCommand.cs:line 90
bei AppMotor.CliApp.CliApplicationExecutor.Execute(String[] args, CancellationToken cancellationToken) in /_/src/AppMotor.CliApp/CliApplicationExecutor.cs:line 403
bei AppMotor.CliApp.CliApplication.RunAsync(String[] args, CancellationToken cancellationToken) in /_/src/AppMotor.CliApp/CliApplication.cs:line 187
".Trim().SplitLines().ToList();

            var expectedCleanedStackTrace = @"
at System.CommandLine.Help.HelpBuilder..ctor(IConsole console, int maxWidth)
at System.CommandLine.CommandLineConfiguration..ctor.λ(BindingContext context)
at System.CommandLine.Invocation.ServiceProvider..ctor.λ(IServiceProvider _)
at System.CommandLine.Invocation.ServiceProvider.GetService(Type serviceType)
at System.CommandLine.Binding.BindingContext.get_HelpBuilder()
at System.CommandLine.Invocation.ParseErrorResult.Apply(InvocationContext context)
at System.CommandLine.Invocation.InvocationPipeline.GetExitCode(InvocationContext context)
at System.CommandLine.Invocation.InvocationPipeline.InvokeAsync(IConsole console)
at System.CommandLine.CommandExtensions.InvokeAsync(Command command, String[] args, IConsole console)
at AppMotor.CliApp.CommandLine.Utils.RootCommandInvoker.InvokeRootCommand(String appDescription, IEnumerable`1 rootSymbols, ICommandHandler commandHandler, ITerminal terminal, String[] args, Func`2 exceptionHandlerFunc) in /_/src/AppMotor.CliApp/CommandLine/Utils/RootCommandInvoker.cs:line 60
at AppMotor.CliApp.CommandLine.CliApplicationWithCommand.Execute(String[] args, CancellationToken cancellationToken) in /_/src/AppMotor.CliApp/CommandLine/CliApplicationWithCommand.cs:line 90
at AppMotor.CliApp.CliApplicationExecutor.Execute(String[] args, CancellationToken cancellationToken) in /_/src/AppMotor.CliApp/CliApplicationExecutor.cs:line 403
at AppMotor.CliApp.CliApplication.RunAsync(String[] args, CancellationToken cancellationToken) in /_/src/AppMotor.CliApp/CliApplication.cs:line 187
".Trim().SplitLines().ToList();

            for (var i = 0; i < testStackTrace.Count; i++)
            {
                yield return new object[]
                {
                    testStackTrace[i],
                    expectedCleanedStackTrace[i],
                };
            }
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Test_Cleanup(string actualStackTraceLine, string expectedStackTraceLine)
    {
        var cleanedStackTraceLines = StackTraceCleaner.CleanupStackTraceLines(new[] { actualStackTraceLine }).ToList();
        cleanedStackTraceLines[0].ShouldBe(expectedStackTraceLine);
    }
}
