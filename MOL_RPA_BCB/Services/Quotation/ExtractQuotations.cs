using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Core.Extesions;

namespace Services.Cotacao
{
    public class ExtractQuotations
    {
        private readonly IWebDriver _driver;
        private readonly ILogger _logger;

        public ExtractQuotations(IWebDriver driver, ILogger logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public List<Models.Quotation> Execute(WebDriverWait wait, string currencyBase)
        {
            // Alternar para o iframe, se necessário
            IWebElement iframe = wait.Until(d => d.FindElement(By.CssSelector("body > app-root > app-root > div > div > main > dynamic-comp > div > div:nth-child(3) > div.col-md-8 > div > iframe")));
            _driver.SwitchTo().Frame(iframe);

            // Localizar a tabela
            IWebElement table = wait.Until(d => d.FindElement(By.CssSelector("table.tabela")));
            var lines = table.FindElements(By.TagName("tr")).Skip(2); // Pular as 2 primeiras linhas de cabeçalho

            var quotations = new List<Models.Quotation>();

            // Iterar pelas linhas da tabela e extrair dados
            foreach (var line in lines)
            {
                var columns = line.FindElements(By.TagName("td"));

                if (columns.Count == 6)
                {
                    string data = columns[0].Text;
                    // string tipo = colunas[1].Text;
                    string tipo = currencyBase;
                    string compra = columns[2].Text;
                    string venda = columns[3].Text;
                    string paridadeCompra = columns[4].Text;
                    string paridadeVenda = columns[5].Text;

                    var quotation = new Models.Quotation
                    {
                        CurrencyName = currencyBase,
                        QuotationBuy = compra.ToDecimal(),
                        QuotationSell = venda.ToDecimal(),
                        ParityBuy = paridadeCompra.ToDecimal(),
                        ParitySell = paridadeVenda.ToDecimal(),
                        Date = data.ToDateTime()
                    };

                    quotations.Add(quotation);
                }
            }
            return quotations;
        }
    }
}