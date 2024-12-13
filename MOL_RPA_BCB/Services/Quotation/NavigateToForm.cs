using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Helpers.Selenium;

namespace Services.Quotation
{
    public class NavigateToForm
    {
        private readonly IWebDriver _driver;
        private readonly ILogger _logger;

        public NavigateToForm(IWebDriver driver, ILogger logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public void Execute(WebDriverWait wait)
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
    }

}
