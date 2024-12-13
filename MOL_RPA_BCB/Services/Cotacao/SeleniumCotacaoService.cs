using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Services.Cotacao
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

        async Task<List<Models.Cotacao>> ICotacaoService.ObterCotacaoesAsync(
            DateTime inicio,
            DateTime fim,
            string moedaBase
        )
        {
            var cotacoes = new List<Models.Cotacao>();

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
                var extract = new ExtractCotations(_driver, _logger);
                cotacoes = extract.Execute(wait, moedaBase);

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
    }
}