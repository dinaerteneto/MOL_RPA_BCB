using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Core.Logging;
using Helpers;
using Models;
using Services.Cotacao;

class Program
{
    static async Task Main(string[] args)
    {
        // Carregar configurações usando o helper
        IConfiguration configuration = ConfigurationHelper.LoadConfiguration();

        // Acessar valores diretamente do appsettings.json
        var baseUrl = configuration["Settings:BaseUrl"];
        var startDate = configuration["Cotation:Params:StartDate"];
        var endDate = configuration["Cotation:Params:EndDate"];
        var currencyName = configuration["Cotation:Params:CurrencyName"];

        // Configuração dos serviços
        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole(); // Adiciona suporte ao log no console
            })
            .AddSingleton<IWebDriver>(provider =>
            {
                var options = new ChromeOptions();
                // options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--no-sandbox");

                return new ChromeDriver(options);

            })
            .AddSingleton(typeof(Core.Logging.ILogger), typeof(DotNetLogger<Program>))
            .AddSingleton<string>(baseUrl) // Registra o baseUrl como uma string
            .AddTransient<ICotacaoService, SeleniumCotacaoService>()
            .BuildServiceProvider();

        // Obter o logger para o Program
        var logger = serviceProvider.GetRequiredService<Core.Logging.ILogger>();
        logger.LogInformation("Aplicação iniciada.");

        // Obter o serviço de cotações
        ICotacaoService? cotacaoService = serviceProvider.GetService<ICotacaoService>();

        if (cotacaoService != null)
        {
            var cotacoes = await cotacaoService.ObterCotacaoesAsync(
                DateTime.Parse(startDate),
                DateTime.Parse(endDate),
                moedaBase: currencyName
            );
            ExibirCotacoes(cotacoes);
        }
    }


    static void ExibirCotacoes(List<Cotacao> cotacoes)
    {
        Console.WriteLine($"{"Data",-15} {"Tipo",-10} {"Compra",-10} {"Venda",-10} {"Paridade Compra",-15} {"Paridade Venda",-15}");
        Console.WriteLine(new string('-', 80));

        // Exibir cada cotação com formatação
        foreach (var cotacao in cotacoes)
        {
            Console.WriteLine($"{cotacao.Data.ToString("dd/MM/yyyy"),-15} "  // Exibindo a data no formato dd/MM/yyyy
                              + $"{cotacao.NomeMoeda,-10} "
                              + $"{cotacao.CotacaoCompra.ToString(),-10} "   // Formatação para 4 casas decimais
                              + $"{cotacao.CotacaoVenda.ToString(),-10} "
                              + $"{cotacao.ParidadeCompra.ToString(),-15} "
                              + $"{cotacao.ParidadeVenda.ToString(),-15}");
        }
    }

}
