using BootstrapBlazorApp2.Server.Data.Models;

namespace BootstrapBlazorApp2.Server.Services
{
    /// <summary>
    /// Interface para o serviço de integração com a API do Tesouro Nacional
    /// </summary>
    public interface ITesouroNacionalService
    {
        /// <summary>
        /// Obtém a lista de anexos de relatórios da API do Tesouro Nacional
        /// </summary>
        /// <returns>Lista de anexos de relatórios</returns>
        Task<List<AnexoRelatorio>> GetAnexosRelatoriosAsync();

        /// <summary>
        /// Obtém os anexos filtrados por esfera
        /// </summary>
        /// <param name="esfera">Esfera para filtrar (C, E, M, U)</param>
        /// <returns>Lista filtrada de anexos</returns>
        Task<List<AnexoRelatorio>> GetAnexosPorEsferaAsync(string esfera);

        /// <summary>
        /// Obtém os anexos filtrados por demonstrativo
        /// </summary>
        /// <param name="demonstrativo">Demonstrativo para filtrar (DCA, QDCC)</param>
        /// <returns>Lista filtrada de anexos</returns>
        Task<List<AnexoRelatorio>> GetAnexosPorDemonstrativoAsync(string demonstrativo);
    }
}

