using DavidIntraDayReport;
using DavidIntraDayReport.Model;
using Serilog;

internal class Program
{
    public static void Main(string[] args)
    {
        var programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(programDataPath, "DavidIntraDayReport", "log.txt"))
            .CreateLogger();

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseSerilog()
            .ConfigureServices((hostContext, services) => {
                IConfiguration configuration = hostContext.Configuration;

                WorkerOptions options = configuration.GetSection("Settings").Get<WorkerOptions>();

                services.AddSingleton(options);

                services.AddHostedService<Worker>();
            });
}