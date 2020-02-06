using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using IAmRaf.MVVM;
using IAmRaf.MVVM.Reactive;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WitUI.Configurations;
using WitUI.ViewModels;

namespace WitUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        [STAThread]
        [System.Diagnostics.DebuggerNonUserCode]
        public static void Main()
        {
            // no App.xaml is needed but it can be used, but in this case
            // - remove this Main() method
            // - remove the StartupUri attribute in App.xaml
            // - load the global resources in xaml
            App app = new App();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.DispatcherUnhandledException += App_DispatcherUnhandledException;
            base.Exit += App_Exit;
            _host = CreateHostBuilder(e.Args).Build();

            base.OnStartup(e);

            var manager = _host.Services.GetRequiredService<ViewModelManager>();
            manager.LoadGlobalResourceDictionary("/Resources/GlobalResources.xaml");

            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("The application is starting");
            manager.Show<MainWindow>();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("The application is closing");
        }

        private void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_host == null)
            {
                MessageBox.Show(e.Exception.ToString());
            }
            else
            {
                var logger = _host.Services.GetRequiredService<ILogger<App>>();
                logger.LogError($"The app crashed: {e.Exception.ToString()}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                //.UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                .ConfigureLogging(options =>
                {
                    options.ClearProviders();
                    options.AddDebug();
                    options.AddFile(cfg =>
                    {
                        cfg.FileName = "log-";
                        cfg.LogDirectory = "WitLogs";
                        cfg.FileSizeLimit = 20 * 1024 * 1024;
                        cfg.Extension = "txt";
                        cfg.Periodicity = NetEscapades.Extensions.Logging.RollingFile.PeriodicityOptions.Daily;
                    });
                })
                .ConfigureAppConfiguration(config =>
                {
                    //config.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // == configurations ==
                    services.Configure<GeneralConfig>(hostContext.Configuration.GetSection("GeneralConfig"));
                    //services.AddSingleton<IPushNotification, PushNotification>();
                    //services.AddScoped<IClientConsumerNotification, ClientNotification>();

                    services.AddSingleton<ViewModelManager>();
                    services.AddBusFactory();

                    // viewmodels
                    services.AddSingleton<MainViewModel>();

                    // views and windows
                    services.AddSingleton<MainView>();
                    services.AddSingleton<MainWindow>();

                    var cfg = new MvvmConfiguration();
                    cfg.AddRange(
                        (typeof(MainWindow), typeof(MainViewModel)),
                        (typeof(MainView), typeof(MainViewModel)));

                    services.AddSingleton(cfg);
                });
        }


    }
}
