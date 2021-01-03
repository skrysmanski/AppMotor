using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.Samples.AppWithVerbs
{
    internal sealed class CloneCommand : CliCommand
    {
        /// <inheritdoc />
        public override string HelpText => "Clone a repository into a new directory";

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private void Execute()
        {

        }
    }
}
