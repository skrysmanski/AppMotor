namespace AppMotor.HttpServer.MvcSample.Services;

public sealed class ServiceSettingsProvider
{
    public int HttpsPort { get; }

    public ServiceSettingsProvider(int httpsPort)
    {
        this.HttpsPort = httpsPort;
    }
}
