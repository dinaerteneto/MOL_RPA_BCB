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
