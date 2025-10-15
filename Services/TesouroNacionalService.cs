using BootstrapBlazorApp2.Server.Data.Models;
using System.Text.Json;

namespace BootstrapBlazorApp2.Server.Services
{
    /// <summary>
    /// Serviço para integração com a API do Tesouro Nacional
    /// </summary>
    public class TesouroNacionalService : ITesouroNacionalService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TesouroNacionalService> _logger;
        private readonly IConfiguration _configuration;

        public TesouroNacionalService(IHttpClientFactory httpClientFactory, ILogger<TesouroNacionalService> logger, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("TesouroNacional");
            _logger = logger;
            _configuration = configuration;
        }

    /// <summary>
    /// Obtém a lista de anexos de relatórios da API do Tesouro Nacional
    /// </summary>
    /// <returns>Lista de anexos de relatórios</returns>
    public async Task<List<AnexoRelatorio>> GetAnexosRelatoriosAsync()
    {
        var maxRetryAttempts = _configuration.GetValue<int>("TesouroNacional:MaxRetryAttempts", 3);
        var useFallback = _configuration.GetValue<bool>("TesouroNacional:UseFallbackData", true);

        for (int attempt = 1; attempt <= maxRetryAttempts; attempt++)
        {
            try
            {
                _logger.LogInformation("Tentativa {Attempt}/{MaxAttempts} de busca de anexos de relatórios na API do Tesouro Nacional", 
                    attempt, maxRetryAttempts);

                var apiUrl = _configuration["TesouroNacional:ApiUrl"] 
                    ?? throw new InvalidOperationException("URL da API do Tesouro Nacional não configurada");

                using var cts = new CancellationTokenSource();
                var timeoutSeconds = _configuration.GetValue<int>("TesouroNacional:TimeoutSeconds", 30);
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                var response = await _httpClient.GetAsync(apiUrl, cts.Token);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync(cts.Token);
                    
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Items != null && apiResponse.Items.Count > 0)
                    {
                        _logger.LogInformation("Sucesso ao obter {Count} anexos da API na tentativa {Attempt}", 
                            apiResponse.Items.Count, attempt);
                        return apiResponse.Items;
                    }
                }

                _logger.LogWarning("Falha ao obter dados da API na tentativa {Attempt}. Status: {StatusCode}", 
                    attempt, response.StatusCode);

                // Se não é a última tentativa, aguarda antes de tentar novamente
                if (attempt < maxRetryAttempts)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)); // Backoff exponencial
                    _logger.LogInformation("Aguardando {Delay} segundos antes da próxima tentativa...", delay.TotalSeconds);
                    await Task.Delay(delay, cts.Token);
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Timeout na tentativa {Attempt} de buscar dados da API do Tesouro Nacional", attempt);
                
                if (attempt == maxRetryAttempts && useFallback)
                {
                    _logger.LogInformation("Usando dados de fallback após {MaxAttempts} tentativas falhadas", maxRetryAttempts);
                    //return GetFallbackData();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na tentativa {Attempt} de buscar anexos de relatórios na API do Tesouro Nacional", attempt);
                
                if (attempt == maxRetryAttempts && useFallback)
                {
                    _logger.LogInformation("Usando dados de fallback após {MaxAttempts} tentativas falhadas", maxRetryAttempts);
                    //return GetFallbackData();
                }
            }
        }

        // Se chegou aqui, todas as tentativas falharam e não há fallback habilitado
        _logger.LogError("Falha ao obter dados da API do Tesouro Nacional após {MaxAttempts} tentativas", maxRetryAttempts);
        return new List<AnexoRelatorio>();
    }

        /// <summary>
        /// Obtém os anexos filtrados por esfera
        /// </summary>
        /// <param name="esfera">Esfera para filtrar (C, E, M, U)</param>
        /// <returns>Lista filtrada de anexos</returns>
        public async Task<List<AnexoRelatorio>> GetAnexosPorEsferaAsync(string esfera)
        {
            var todosAnexos = await GetAnexosRelatoriosAsync();
            return todosAnexos.Where(a => a.Esfera.Equals(esfera, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Obtém os anexos filtrados por demonstrativo
        /// </summary>
        /// <param name="demonstrativo">Demonstrativo para filtrar (DCA, QDCC)</param>
        /// <returns>Lista filtrada de anexos</returns>
        public async Task<List<AnexoRelatorio>> GetAnexosPorDemonstrativoAsync(string demonstrativo)
        {
            var todosAnexos = await GetAnexosRelatoriosAsync();
            return todosAnexos.Where(a => a.Demonstrativo.Equals(demonstrativo, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Retorna dados de fallback quando a API está indisponível
        /// </summary>
        /// <returns>Lista de anexos de exemplo</returns>
        /*private List<AnexoRelatorio> GetFallbackData()
        {
            _logger.LogInformation("Retornando dados de fallback para anexos de relatórios");
            
            return new List<AnexoRelatorio>
            {
                // Contábil
                new() { Esfera = "C", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 01" },
                new() { Esfera = "C", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 05" },
                new() { Esfera = "C", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 06" },
                new() { Esfera = "C", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 01" },
                new() { Esfera = "C", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 02" },

                // Estadual
                new() { Esfera = "E", Demonstrativo = "DCA", Anexo = "Anexo I-AB" },
                new() { Esfera = "E", Demonstrativo = "DCA", Anexo = "Anexo I-C" },
                new() { Esfera = "E", Demonstrativo = "DCA", Anexo = "Anexo I-D" },
                new() { Esfera = "E", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 01" },
                new() { Esfera = "E", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 02" },
                new() { Esfera = "E", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 03" },
                new() { Esfera = "E", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 01" },
                new() { Esfera = "E", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 02" },

                // Municipal
                new() { Esfera = "M", Demonstrativo = "DCA", Anexo = "Anexo I-AB" },
                new() { Esfera = "M", Demonstrativo = "DCA", Anexo = "Anexo I-C" },
                new() { Esfera = "M", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 01" },
                new() { Esfera = "M", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 02" },
                new() { Esfera = "M", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 01" },
                new() { Esfera = "M", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 02" },

                // União
                new() { Esfera = "U", Demonstrativo = "DCA", Anexo = "DCA-Anexo I-AB" },
                new() { Esfera = "U", Demonstrativo = "DCA", Anexo = "DCA-Anexo I-C" },
                new() { Esfera = "U", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 01" },
                new() { Esfera = "U", Demonstrativo = "QDCC", Anexo = "RGF-Anexo 02" },
                new() { Esfera = "U", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 01" },
                new() { Esfera = "U", Demonstrativo = "QDCC", Anexo = "RREO-Anexo 02" }
            };
        }*/
    }
}
