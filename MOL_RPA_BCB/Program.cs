using Models;
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
