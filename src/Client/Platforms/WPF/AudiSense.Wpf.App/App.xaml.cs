using System.IO;
using System.Net.Http;
using System.Windows;

using AudiSense.Client.Shared.Services.Interfaces;
using AudiSense.Wpf.Core.Services;
using AudiSense.Wpf.Core.ViewModels;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace AudiSense.Wpf.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/AudiSense-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            _host = CreateHostBuilder(e.Args).Build();
            await _host.StartAsync();

            // Show main window
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            MessageBox.Show($"Application failed to start: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                      .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Register application services
                services.AddSingleton<MainWindow>();

                // Register HTTP client with configuration
                services.AddHttpClient<HttpDataService>(client =>
                {
                    var timeout = context.Configuration.GetValue<int>("ApiSettings:Timeout", 30);
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();

                    // For development only - bypass SSL certificate validation for localhost
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        if (message.RequestUri?.Host == "localhost" || message.RequestUri?.Host == "127.0.0.1")
                        {
                            return true; // Accept any certificate for localhost
                        }
                        return errors == System.Net.Security.SslPolicyErrors.None;
                    };

                    return handler;
                });

                // Register platform-specific services
                services.AddSingleton<IDataService, HttpDataService>();
                services.AddSingleton<IHearingTestApiService, HearingTestApiService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Register view models
                services.AddSingleton<HearingTestsViewModel>();
            });
}
