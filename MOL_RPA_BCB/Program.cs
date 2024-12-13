using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Core.Logging;
using Helpers;
using Services.Quotation;
using Models;
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
                //options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--no-sandbox");

                return new ChromeDriver(options);

            })
            .AddSingleton(typeof(Core.Logging.ILogger), typeof(DotNetLogger<Program>))
            .AddSingleton<string>(baseUrl) // Registra o baseUrl como uma string
            .AddTransient<IQuotationService, SeleniumQuotationService>()
            .BuildServiceProvider();

        // Obter o logger para o Program'
        var logger = serviceProvider.GetRequiredService<Core.Logging.ILogger>();
        logger.LogInformation("Aplicação iniciada.");

        // Obter o serviço de cotações
        IQuotationService? quotationService = serviceProvider.GetService<IQuotationService>();

        if (quotationService != null)
        {
            var quotations = await quotationService.GetQuotationsAsync(
                DateTime.Parse(startDate),
                DateTime.Parse(endDate),
                currencyBase: currencyName
            );
            ShowQuotations(quotations);
        }
    }


    static void ShowQuotations(List<Quotation> quotations)
    {
        Console.WriteLine($"{"Data",-15} {"Tipo",-10} {"Compra",-10} {"Venda",-10} {"Paridade Compra",-15} {"Paridade Venda",-15}");
        Console.WriteLine(new string('-', 80));

        // Exibir cada cotação com formatação
        foreach (var quotation in quotations)
        {
            Console.WriteLine($"{quotation.Date.ToString("dd/MM/yyyy"),-15} "  // Exibindo a data no formato dd/MM/yyyy
                              + $"{quotation.CurrencyName,-10} "
                              + $"{quotation.QuotationBuy.ToString(),-10} "   // Formatação para 4 casas decimais
                              + $"{quotation.QuotationSell.ToString(),-10} "
                              + $"{quotation.ParityBuy.ToString(),-15} "
                              + $"{quotation.ParitySell.ToString(),-15}");
        }
    }

}
