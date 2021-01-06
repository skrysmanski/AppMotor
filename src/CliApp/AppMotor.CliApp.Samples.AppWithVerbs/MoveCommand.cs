using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.Samples.AppWithVerbs
{
    internal class MoveCommand : GitCommandBase
    {
        /// <inheritdoc />
        public override string HelpText => "Move or rename a file, a directory, or a symlink";

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private void Execute()
        {

        }
    }
}
