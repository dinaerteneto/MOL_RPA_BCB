using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Helpers.Selenium;

namespace Services.Quotation
{
    public class SubmitForm
    {
        private readonly IWebDriver _driver;
        private readonly ILogger _logger;

        public SubmitForm(IWebDriver driver, ILogger logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public void Execute(WebDriverWait wait, DateTime inicio, DateTime fim, string moedaBase)
        {
            Step1_ChangeToIframe(wait);
            Step2_SelectRadioButton(wait);
            Step3_FillDates(wait, inicio, fim);
            Step4_SelectCurrency(wait, moedaBase);
            Step5_SubmitForm(wait);
        }


        private void Step1_ChangeToIframe(WebDriverWait wait)
        {
            // Trocar para o iframe
            IWebElement iframe = wait.Until(d => d.FindElement(By.CssSelector("body > app-root > app-root > div > div > main > dynamic-comp > div > div:nth-child(3) > div.col-md-8 > div > iframe")));
            _driver.SwitchTo().Frame(iframe);
        }

        private void Step2_SelectRadioButton(WebDriverWait wait)
        {
            // Selecionar o radio button
            IWebElement radioButton = wait.Until(d => d.FindElement(By.CssSelector("input[type='radio'][name='RadOpcao'][value='1']")));
            radioButton.Click();
            _logger.LogDebug("Primeira opção do radio button selecionada!");
        }

        private void Step3_FillDates(WebDriverWait wait, DateTime startDate, DateTime endDate)
        {
            // Preencher datas
            var initialDate = startDate.ToString("ddMMyyyy");
            var finalDate = endDate.ToString("ddMMyyyy");
            SeleniumHelper.FillInputField(wait, "#DATAINI", initialDate);
            _logger.LogDebug($"Data inicial digitida: {initialDate}");
            SeleniumHelper.FillInputField(wait, "#DATAFIM", finalDate);
            _logger.LogDebug($"Data final digitida: {finalDate}");
            _logger.LogDebug("Datas preenchidas!");
        }

        private void Step4_SelectCurrency(WebDriverWait wait, string currencyBase)
        {
            // Selecionar moeda
            IWebElement moedaInput = wait.Until(d => d.FindElement(By.CssSelector("body > div > form > table:nth-child(3) > tbody > tr:nth-child(4) > td:nth-child(2) > select")));
            moedaInput.Click();
            moedaInput.SendKeys(currencyBase);
            moedaInput.SendKeys(Keys.Enter);
            _logger.LogDebug("Moeda selecionada!");
        }

        private void Step5_SubmitForm(WebDriverWait wait)
        {
            // Submeter formulário
            SeleniumHelper.ClickElement(wait, "body > div > form > div > input");
            _logger.LogDebug("Formulário submetido!");

            // Voltar ao contexto principal
            _driver.SwitchTo().DefaultContent();
        }

    }
}
