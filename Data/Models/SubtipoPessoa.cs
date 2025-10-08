using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BootstrapBlazorApp2.Server.Data.Models
{
    public class SubtipoPessoa
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;

        // Chave estrangeira
        [ForeignKey(nameof(TipoPessoa))]
        public int TipoPessoaId { get; set; }

        // Propriedade de navegação
        public virtual TipoPessoa TipoPessoa { get; set; } = null!;
    }
}
