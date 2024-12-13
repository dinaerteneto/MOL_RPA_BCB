namespace Core
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Carrega a configuração do arquivo appsettings.json.
        /// </summary>
        /// <returns>Instância de IConfiguration carregada com as configurações do JSON.</returns>
        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
