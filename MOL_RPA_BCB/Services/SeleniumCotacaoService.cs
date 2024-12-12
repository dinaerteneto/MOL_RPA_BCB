using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Models;
using Helpers;
using Helpers.Selenium;

namespace Services
{
    public class SeleniumCotacaoService : ICotacaoService
    {
        private readonly IWebDriver _driver;
        public SeleniumCotacaoService(IWebDriver driver)
        {
            _driver = driver;
        }

        async Task<List<Cotacao>> ICotacaoService.ObterCotacaoesAsync(DateTime inicio, DateTime fim, string moedaBase)
        {
            var cotacoes = new List<Cotacao>();

            try
            {
                // navegar para o site  
                _driver.Navigate().GoToUrl("https://www.bcb.gov.br/");
                _driver.Manage().Window.Maximize();

                // implementar navegaçõe e extração dos dados usando Selenium
                // Aguarde o carregamento inicial
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));

                NavegarParaFormulario(wait);
                SubmeterFormulario(wait, inicio, fim, moedaBase);
                return ExtrairCotacoes(wait, moedaBase);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar cotação: {ex.Message}");
            }
            finally
            {
                if (_driver != null)
                {
                    _driver.Quit();
                    _driver.Dispose();
                }
            }

            return await Task.FromResult(cotacoes);
        }

        /// <summary>
        /// Navega pelo menu para abrir a página que contém o formulário
        /// </summary>
        /// <param name = "wait" > Instância do WebDriverWait.</param>
        private void NavegarParaFormulario(WebDriverWait wait)
        {
            var menuSteps = new List<(string Selector, string Description)>
        {
            ("#navbarDropdown2", "Estabilidade financeira"),
            ("#navbarSupportedContent > ul > li.nav-item.dropdown.show > div > div > div > div.col-3.nivel.segundo-nivel > div > div > a:nth-child(5)", "Câmbio e Capitais internacionais"),
            ("#navbarSupportedContent > ul > li.nav-item.dropdown.show > div > div > div > div:nth-child(6) > div > div > a:nth-child(2)", "Cotação de moedas"),
            ("#navbarSupportedContent > ul > li.nav-item.dropdown.show > div > div > div > div:nth-child(38) > div > div > a:nth-child(4)", "Consulta de cotações e boletins")
        };

            foreach (var (selector, description) in menuSteps)
            {
                Console.WriteLine($"Clicando em: {description}");
                SeleniumHelper.ClickElement(wait, selector);
            }

            Console.WriteLine("Todos os menus navegados com sucesso!");
        }

        /// <summary>
        /// Método auxiliar que combina a manipulação dos elementos do formulário 
        /// para submete-lo
        /// </summary>
        /// <param name="wait">Instância do WebDriverWait.</param>
        /// <param name="inicio">Data inicial que será digitada no formulário</param>
        /// <param name="fim">Data final que será digitada no formulário</param>
        /// <param name="moedaBase">Nome da moeda a ser pesquisada</param>
        private void SubmeterFormulario(WebDriverWait wait, DateTime inicio, DateTime fim, string moedaBase)
        {
            // Trocar para o iframe
            IWebElement iframe = wait.Until(d => d.FindElement(By.CssSelector("body > app-root > app-root > div > div > main > dynamic-comp > div > div:nth-child(3) > div.col-md-8 > div > iframe")));
            _driver.SwitchTo().Frame(iframe);

            Console.WriteLine("Contexto alterado para o iframe.");

            // Selecionar o radio button
            IWebElement radioButton = wait.Until(d => d.FindElement(By.CssSelector("input[type='radio'][name='RadOpcao'][value='1']")));
            radioButton.Click();
            Console.WriteLine("Primeira opção do radio button selecionada!");

            // Preencher datas
            var dataInicial = inicio.ToString("ddMMyyyy");
            var dataFinal = fim.ToString("ddMMyyyy");
            SeleniumHelper.FillInputField(wait, "#DATAINI", dataInicial);
            Console.WriteLine($"Data inicial digitida: {dataInicial}");
            SeleniumHelper.FillInputField(wait, "#DATAFIM", dataFinal);
            Console.WriteLine($"Data final digitida: {dataFinal}");
            Console.WriteLine("Datas preenchidas!");

            // Selecionar moeda
            IWebElement moedaInput = wait.Until(d => d.FindElement(By.CssSelector("body > div > form > table:nth-child(3) > tbody > tr:nth-child(4) > td:nth-child(2) > select")));
            moedaInput.Click();
            moedaInput.SendKeys(moedaBase);
            moedaInput.SendKeys(Keys.Enter);
            Console.WriteLine("Moeda selecionada!");

            // Submeter formulário
            SeleniumHelper.ClickElement(wait, "body > div > form > div > input");
            Console.WriteLine("Formulário submetido!");

            // Voltar ao contexto principal
            _driver.SwitchTo().DefaultContent();
            Console.WriteLine("Contexto alterado de volta para o conteúdo principal.");
        }

        /// <summary>
        /// Método responsável por extrair os dados da tabela para o domínio
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="moedaBase"></param>
        private List<Cotacao> ExtrairCotacoes(WebDriverWait wait, string moedaBase)
        {
            // Alternar para o iframe, se necessário
            IWebElement iframe = wait.Until(d => d.FindElement(By.CssSelector("body > app-root > app-root > div > div > main > dynamic-comp > div > div:nth-child(3) > div.col-md-8 > div > iframe")));
            _driver.SwitchTo().Frame(iframe);

            Console.WriteLine("Contexto alterado para o iframe.");

            // Localizar a tabela
            IWebElement tabela = wait.Until(d => d.FindElement(By.CssSelector("table.tabela")));
            var linhas = tabela.FindElements(By.TagName("tr")).Skip(2); // Pular as 2 primeiras linhas de cabeçalho

            var cotacoes = new List<Cotacao>();
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

                    var cotacao = new Cotacao
                    {
                        NomeMoeda = moedaBase,
                        CotacaoCompra = Formats.ConvertToDecimal(compra),
                        CotacaoVenda = Formats.ConvertToDecimal(venda),
                        ParidadeCompra = Formats.ConvertToDecimal(paridadeCompra),
                        ParidadeVenda = Formats.ConvertToDecimal(paridadeVenda),
                        Data = Formats.ConvertToDateTime(data)
                    };

                    cotacoes.Add(cotacao);
                }
            }
            return cotacoes;
        }

    }

}