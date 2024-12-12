namespace Helpers.Logging
{
    public static class Logger
    {
        private static string logFilePath = "log.txt"; // Caminho do arquivo de log

        /// <summary>
        /// Exibe uma mensagem no console e, se configurado, escreve no arquivo de log.
        /// </summary>
        /// <param name="message">A mensagem a ser registrada.</param>
        /// <param name="logToFile">Se verdadeiro, a mensagem será registrada em um arquivo de log.</param>
        public static void Log(string message, bool logToFile = true)
        {
            // Exibe no console
            Console.WriteLine(message);

            // Se logToFile for verdadeiro, grava no arquivo de log
            if (logToFile)
            {
                try
                {
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao gravar no arquivo de log: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Exibe uma mensagem de erro no console.
        /// </summary>
        /// <param name="message">A mensagem de erro.</param>
        public static void LogError(string message)
        {
            var errorMessage = $"[ERRO] {message}";
            Log(errorMessage); // Exibe no console e no arquivo de log
        }

        /// <summary>
        /// Exibe uma mensagem de informação no console.
        /// </summary>
        /// <param name="message">A mensagem de informação.</param>
        public static void LogInfo(string message)
        {
            var infoMessage = $"[INFO] {message}";
            Log(infoMessage); // Exibe no console e no arquivo de log
        }

        /// <summary>
        /// Exibe uma mensagem de sucesso no console.
        /// </summary>
        /// <param name="message">A mensagem de sucesso.</param>
        public static void LogSuccess(string message)
        {
            var successMessage = $"[SUCESSO] {message}";
            Log(successMessage); // Exibe no console e no arquivo de log
        }
    }
}
