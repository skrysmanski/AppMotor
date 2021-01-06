using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.Samples.AppWithVerbs
{
    internal class AddCommand : GitCommandBase
    {
        /// <inheritdoc />
        public override string HelpText => "Add file contents to the index";

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private void Execute()
        {

        }
    }
}
