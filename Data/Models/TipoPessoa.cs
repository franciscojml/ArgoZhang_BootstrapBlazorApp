using System.ComponentModel.DataAnnotations;

namespace BootstrapBlazorApp2.Server.Data.Models
{
    public class TipoPessoa
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Descricao { get; set; } = string.Empty;

        // Propriedade de navegação para SubtiposPessoa
        public virtual ICollection<SubtipoPessoa> SubtiposPessoa { get; set; } = new List<SubtipoPessoa>();
    }
}
