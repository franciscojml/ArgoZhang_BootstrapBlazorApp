using System.ComponentModel.DataAnnotations;

namespace BootstrapBlazorApp2.Server.Data.Models
{
    /// <summary>
    /// Modelo para representar os anexos de relatórios da API do Tesouro Nacional
    /// </summary>
    public class AnexoRelatorio
    {
        /// <summary>
        /// Esfera (C=Contábil, E=Estadual, M=Municipal, U=União)
        /// </summary>
        public string Esfera { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de demonstrativo (DCA, QDCC)
        /// </summary>
        public string Demonstrativo { get; set; } = string.Empty;

        /// <summary>
        /// Nome do anexo
        /// </summary>
        public string Anexo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição completa para exibição no Select
        /// </summary>
        public string DescricaoCompleta => $"{Esfera} - {Demonstrativo} - {Anexo}";

        /// <summary>
        /// Valor único para identificação (usado como Value no SelectedItem)
        /// </summary>
        public string Valor => $"{Esfera}|{Demonstrativo}|{Anexo}";
    }

    /// <summary>
    /// Modelo para a resposta completa da API
    /// </summary>
    public class ApiResponse
    {
        public List<AnexoRelatorio> Items { get; set; } = new();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
        public List<ApiLink> Links { get; set; } = new();
    }

    /// <summary>
    /// Modelo para os links da API
    /// </summary>
    public class ApiLink
    {
        public string Rel { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
    }
}

