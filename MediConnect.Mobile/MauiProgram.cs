using MediConnect.Mobile.Services;
using MediConnect.Mobile.ViewModels;
using MediConnect.Mobile.Views;

namespace MediConnect.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();


        // Services
        builder.Services.AddSingleton<SessionService>();

        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<VitalsService>();
        builder.Services.AddSingleton<RecordsService>();


        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<RecordsViewModel>();
        builder.Services.AddTransient<AddRecordViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();        
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<RecordsPage>();
        builder.Services.AddTransient<AddRecordPage>();
        builder.Services.AddTransient<TriagePage>()

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}


