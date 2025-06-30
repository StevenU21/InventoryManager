using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using InventoryManager.Data;
using InventoryManager.Services; // Agrega el using para los servicios

namespace InventoryManager
{
    public static class MauiProgram
    {
        public static IServiceProvider? Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                string dbPath = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "system.db"
                );
                options.UseSqlite($"Data Source={dbPath}");
            });
            builder.Services.AddSingleton<AppShell>();

            // Registro de servicios
            builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            builder.Services.AddScoped(typeof(IValidationService<>), typeof(ValidationService<>));
            builder.Services.AddSingleton<IMessagingService, MessagingService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            Services = app.Services;

            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }
            return app;
        }
    }
}
