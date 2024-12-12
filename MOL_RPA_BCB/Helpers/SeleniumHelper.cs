using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Helpers.Selenium
{
    public static class SeleniumHelper
    {

        /// <summary>
        /// Método auxiliar para clicar em um elemento pelo seletor CSS.
        /// </summary>
        /// <param name="wait">Instância do WebDriverWait.</param>
        /// <param name="cssSelector">Seletor CSS do elemento.</param>
        public static void ClickElement(WebDriverWait wait, string cssSelector)
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
        public static void FillInputField(WebDriverWait wait, string cssSelector, string value)
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
}
