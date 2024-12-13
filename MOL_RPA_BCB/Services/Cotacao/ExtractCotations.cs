using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Helpers.Selenium;
using Helpers.Extensions;

namespace Services.Cotacao
{
    public class ExtractCotations
    {
        private readonly IWebDriver _driver;
        private readonly ILogger _logger;

        public ExtractCotations(IWebDriver driver, ILogger logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public List<Models.Cotacao> Execute(WebDriverWait wait, string moedaBase)
        {
            // Alternar para o iframe, se necessário
            IWebElement iframe = wait.Until(d => d.FindElement(By.CssSelector("body > app-root > app-root > div > div > main > dynamic-comp > div > div:nth-child(3) > div.col-md-8 > div > iframe")));
            _driver.SwitchTo().Frame(iframe);

            // Localizar a tabela
            IWebElement tabela = wait.Until(d => d.FindElement(By.CssSelector("table.tabela")));
            var linhas = tabela.FindElements(By.TagName("tr")).Skip(2); // Pular as 2 primeiras linhas de cabeçalho

            var cotacoes = new List<Models.Cotacao>();
            var culturaInvariante = System.Globalization.CultureInfo.InvariantCulture;

            // Iterar pelas linhas da tabela e extrair dados
            foreach (var linha in linhas)
            {
                var colunas = linha.FindElements(By.TagName("td"));

                if (colunas.Count == 6)
                {
                    string data = colunas[0].Text;
                    // string tipo = colunas[1].Text;
                    string tipo = moedaBase;
                    string compra = colunas[2].Text;
                    string venda = colunas[3].Text;
                    string paridadeCompra = colunas[4].Text;
                    string paridadeVenda = colunas[5].Text;

                    var cotacao = new Models.Cotacao
                    {
                        NomeMoeda = moedaBase,
                        CotacaoCompra = compra.ToDecimal(),
                        CotacaoVenda = venda.ToDecimal(),
                        ParidadeCompra = paridadeCompra.ToDecimal(),
                        ParidadeVenda = paridadeVenda.ToDecimal(),
                        Data = data.ToDateTime()
                    };

                    cotacoes.Add(cotacao);
                }
            }
            return cotacoes;
        }
    }
}