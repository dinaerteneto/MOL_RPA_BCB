using System.Globalization;
using Helpers.Exceptions;

namespace Helpers
{
    public static class Formats
    {
        /// <summary>
        /// Converte uma string para DateTime usando o formato brasileiro.
        /// </summary>
        /// <param name="dataString">A string representando a data.</param>
        /// <returns>Retorna a data convertida para DateTime.</returns>
        public static DateTime ConvertToDateTime(string dataString)
        {
            // Usando o formato brasileiro (dia/mês/ano)
            DateTime result;
            if (DateTime.TryParseExact(dataString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            else
            {
                // Se a conversão falhar, podemos lançar uma exceção personalizada ou retornar DateTime.MinValue
                throw new HelperException("Data inválida fornecida para conversão.");
            }
        }

        /// <summary>
        /// Converte uma string para decimal, substituindo a vírgula por ponto.
        /// </summary>
        /// <param name="numeroString">A string representando o número.</param>
        /// <returns>Retorna o número convertido para decimal.</returns>
        public static decimal ConvertToDecimal(string numeroString)
        {
            // Substituindo a vírgula por ponto para conversão correta
            decimal result;
            if (decimal.TryParse(numeroString.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            else
            {
                throw new HelperException("Número inválido fornecido para conversão.");
            }
        }

    }
}

