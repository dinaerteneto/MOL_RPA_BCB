using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Helpers.Selenium;
using Models;
using Helpers.Extensions;

namespace Services
{
    public class SeleniumCotacaoService : ICotacaoService
    {
        private readonly IWebDriver _driver;
        private readonly ILogger<SeleniumCotacaoService> _logger;
        private readonly string _baseUrl;

        public SeleniumCotacaoService(IWebDriver driver, ILogger<SeleniumCotacaoService> logger, string baseUrl)
        {
            _driver = driver;
            _logger = logger;
            _baseUrl = baseUrl;
        }

        async Task<List<Cotacao>> ICotacaoService.ObterCotacaoesAsync(
            DateTime inicio,
            DateTime fim, 
            string moedaBase
        )
        {
            var cotacoes = new List<Cotacao>();

            try
            {
                // navegar para o site  
                _driver.Navigate().GoToUrl(_baseUrl);
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
                _logger.LogError($"Ao processar cotação: {ex.Message}");
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
                _logger.LogDebug($"Clicando em: {description}");
                SeleniumHelper.ClickElement(wait, selector);
            }

            _logger.LogDebug("Todos os menus navegados com sucesso!");
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

            // Selecionar o radio button
            IWebElement radioButton = wait.Until(d => d.FindElement(By.CssSelector("input[type='radio'][name='RadOpcao'][value='1']")));
            radioButton.Click();
            _logger.LogDebug("Primeira opção do radio button selecionada!");

            // Preencher datas
            var dataInicial = inicio.ToString("ddMMyyyy");
            var dataFinal = fim.ToString("ddMMyyyy");
            SeleniumHelper.FillInputField(wait, "#DATAINI", dataInicial);
            _logger.LogDebug($"Data inicial digitida: {dataInicial}");
            SeleniumHelper.FillInputField(wait, "#DATAFIM", dataFinal);
            _logger.LogDebug($"Data final digitida: {dataFinal}");
            _logger.LogDebug("Datas preenchidas!");

            // Selecionar moeda
            IWebElement moedaInput = wait.Until(d => d.FindElement(By.CssSelector("body > div > form > table:nth-child(3) > tbody > tr:nth-child(4) > td:nth-child(2) > select")));
            moedaInput.Click();
            moedaInput.SendKeys(moedaBase);
            moedaInput.SendKeys(Keys.Enter);
            _logger.LogDebug("Moeda selecionada!");

            // Submeter formulário
            SeleniumHelper.ClickElement(wait, "body > div > form > div > input");
            _logger.LogDebug("Formulário submetido!");

            // Voltar ao contexto principal
            _driver.SwitchTo().DefaultContent();
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