using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.Samples.AppWithVerbs
{
    internal class MoveCommand : CliCommand
    {
        /// <inheritdoc />
        public override string HelpText => "Create an empty Git repository or reinitialize an existing one";

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private void Execute()
        {

        }
    }
}
