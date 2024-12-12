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
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            // Navegar pelos menus usando o método reutilizável
            ClickElement(wait, "#navbarDropdown2"); // Estabilidade financeira
            ClickElement(wait, "#navbarSupportedContent > ul > li.nav-item.dropdown.show > div > div > div > div.col-3.nivel.segundo-nivel > div > div > a:nth-child(5)"); // Câmbio e Capitais internacionais
            ClickElement(wait, "#navbarSupportedContent > ul > li.nav-item.dropdown.show > div > div > div > div:nth-child(6) > div > div > a:nth-child(2)"); // Cotação de moedas
            ClickElement(wait, "#navbarSupportedContent > ul > li.nav-item.dropdown.show > div > div > div > div:nth-child(38) > div > div > a:nth-child(4)"); // Consulta de cotações e boletins

            Console.WriteLine("Todos os menus foram clicados com sucesso!");

            // Certifique-se de que chegou na página correta
            Console.WriteLine("Título da página atual: " + _driver.Title);

            Console.WriteLine("Todos os menus foram clicados com sucesso!");

            // Certifique-se de que chegou na página correta
            Console.WriteLine("Título da página atual: " + _driver.Title);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar cotação: {ex.Message}");
        }
        finally {
            if (_driver != null) { 
                // _driver.Quit();
                // _driver.Dispose();
            }
        }
        
        return await Task.FromResult( cotacoes );
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

}
