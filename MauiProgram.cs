using Microsoft.Extensions.Logging;

namespace RaceTo21_MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Tickerbit-mono.otf", "BitMono");
				fonts.AddFont("Tickerbit-regular.otf", "BitRegular");
				fonts.AddFont("Tickerbit-italic.otf", "BitItalic");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
