using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;

namespace BilleteraDigital
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //SyncfusionLicenseProvider.RegisterLicense("@32392e302e303b32393bCcc7Mae+oozzryS8ZnL+XH6otQcfq7+nqDFEZuCBUCM=\r\n");
            //SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF1cWWhPYVF0WmFZfVtgfF9GaFZSQGY/P1ZhSXxWdkBhWn1ecH1RQ2VZVkF9XUs=");
            SyncfusionLicenseProvider.RegisterLicense("Mzg3NzI0NEAzMjM5MmUzMDJlMzAzYjMyMzkzYkJPMllkeXhXOGhOV3FCYjN3c1pMVFp4MHhJWGoxY0lPSWwrRVVRNE1kUzA9");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMicrocharts()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesomeSolid.otf", "AwesomeSolid");
                });
            builder.Services.AddSingleton<Utilitario.DatabaseService>();
            builder.Services.AddTransient<Views.FormularioRegistro>();
            builder.Services.AddTransient<Views.vInicio>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
