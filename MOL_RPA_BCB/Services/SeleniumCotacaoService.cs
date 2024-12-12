namespace Services;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Models;

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
            cotacoes = ExtrairCotacoes(wait, moedaBase);
            ExibirCotacoes(cotacoes);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar cotação: {ex.Message}");
        }
        finally {
            if (_driver != null) { 
                _driver.Quit();
                _driver.Dispose();
            }
        }
        
        return await Task.FromResult( cotacoes );
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
            ClickElement(wait, selector);
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
        FillInputField(wait, "#DATAINI", dataInicial);
        Console.WriteLine($"Data inicial digitida: {dataInicial}");
        FillInputField(wait, "#DATAFIM", dataFinal);
        Console.WriteLine($"Data final digitida: {dataFinal}");
        Console.WriteLine("Datas preenchidas!");

        // Selecionar moeda
        IWebElement moedaInput = wait.Until(d => d.FindElement(By.CssSelector("body > div > form > table:nth-child(3) > tbody > tr:nth-child(4) > td:nth-child(2) > select")));
        moedaInput.Click();
        moedaInput.SendKeys(moedaBase);
        moedaInput.SendKeys(Keys.Enter);
        Console.WriteLine("Moeda selecionada!");

        // Submeter formulário
        ClickElement(wait, "body > div > form > div > input");
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

                // Tentativa de conversão da data para DateTime usando o formato dd/MM/yyyy
                DateTime dataConvertida;
                bool dataValida = DateTime.TryParseExact(data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);

                if (!dataValida)
                {
                    // Caso a data não seja válida, você pode lançar uma exceção ou tratar o erro de forma adequada.
                    throw new FormatException($"Data inválida: {data}");
                }


                var cotacao = new Cotacao
                {
                    NomeMoeda = moedaBase,
                    CotacaoCompra = decimal.Parse(compra.Replace(",", "."), culturaInvariante),
                    CotacaoVenda = decimal.Parse(venda.Replace(",", "."), culturaInvariante),
                    ParidadeCompra = decimal.Parse(paridadeCompra.Replace(",", "."), culturaInvariante),
                    ParidadeVenda = decimal.Parse(paridadeVenda.Replace(",", "."), culturaInvariante),
                    Data = dataConvertida
                };

                cotacoes.Add(cotacao);
            }
        }
        return cotacoes;
    }



    private void ExibirCotacoes(List<Cotacao> cotacoes)
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



    /// <summary>
    /// Método auxiliar para clicar em um elemento pelo seletor CSS.
    /// </summary>
    /// <param name="wait">Instância do WebDriverWait.</param>
    /// <param name="cssSelector">Seletor CSS do elemento.</param>
    private void ClickElement(WebDriverWait wait, string cssSelector)
    {
        try
        {
            IWebElement element = wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
            element.Click();
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine($"Elemento com seletor '{cssSelector}' não foi encontrado dentro do tempo limite.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao clicar no elemento com seletor '{cssSelector}': {ex.Message}");
            throw;
        }
    }


    /// <summary>
    /// Método para preencher campos de input
    /// </summary>
    /// <param name="wait">Instância do WebDriverWait.</param>
    /// <param name="cssSelector">Seletor CSS do elemento.</param>
    /// <param name="value">Valor a ser inserido no input</param>
    private void FillInputField(WebDriverWait wait, string cssSelector, string value)
    {
        try
        {
            IWebElement inputField = wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
            inputField.Clear();
            inputField.SendKeys(value);
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine($"Campo de entrada com seletor '{cssSelector}' não foi encontrado dentro do tempo limite.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao preencher o campo de entrada com seletor '{cssSelector}': {ex.Message}");
            throw;
        }
    }

}
