using JetBrains.Annotations;

namespace AppMotor.HttpServer.WebApiSample.Models;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class TodoItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}
