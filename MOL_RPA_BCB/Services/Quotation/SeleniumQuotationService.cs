using Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Services.Cotacao;

namespace Services.Quotation
{
    public class SeleniumQuotationService : IQuotationService
    {
        private readonly IWebDriver _driver;
        private readonly ILogger<SeleniumQuotationService> _logger;
        private readonly string _baseUrl;

        public SeleniumQuotationService(IWebDriver driver, ILogger<SeleniumQuotationService> logger, string baseUrl)
        {
            _driver = driver;
            _logger = logger;
            _baseUrl = baseUrl;
        }

        async Task<List<Models.Quotation>> IQuotationService.GetQuotationsAsync(
            DateTime inicio,
            DateTime fim,
            string moedaBase
        )
        {
            var cotacoes = new List<Models.Quotation>();

            try
            {
                // navegar para o site  
                _driver.Navigate().GoToUrl(_baseUrl);
                _driver.Manage().Window.Maximize();

                // Aguarde o carregamento inicial
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));

                // Navegar para o formulário
                var navigation = new NavigateToForm(_driver, _logger);
                navigation.Execute(wait);

                // Preencher o formulário
                var submit = new SubmitForm(_driver, _logger);
                submit.Execute(wait, inicio, fim, moedaBase);

                // Extrair as cotações
                var extract = new ExtractQuotations(_driver, _logger);
                cotacoes = extract.Execute(wait, moedaBase);

            }
            catch (WebDriverException ex)
            {
                var message = $" WebDriver error on process quotations: {ex.Message}";
                _logger.LogError(message);
                throw new AppException(message, ex);
            }
            catch (Exception ex)
            {
                var message = $"unexpected error while extracting quotations: {ex.Message}";
                _logger.LogError(message);
                throw new AppException(message, ex); ;
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
    }
}