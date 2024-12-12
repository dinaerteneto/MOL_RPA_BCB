using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Services;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IWebDriver, ChromeDriver>()
            .AddTransient<ICotacaoService, SeleniumCotacaoService>()
            .BuildServiceProvider();

        ICotacaoService? cotacaoService = serviceProvider.GetService<ICotacaoService>();

        if (cotacaoService != null)
        {
            var cotacoes = await cotacaoService.ObterCotacaoesAsync(
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 5),
                "Euro"
            );
        }
    }
}
