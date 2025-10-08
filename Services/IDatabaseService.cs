using BootstrapBlazorApp2.Server.Data.Models;

namespace BootstrapBlazorApp2.Server.Services
{
    public interface IDatabaseService
    {
        Task<List<TipoPessoa>> GetTiposPessoaAsync();
        Task<List<SubtipoPessoa>> GetSubtiposPessoaAsync();
        Task<List<SubtipoPessoa>> GetSubtiposByTipoAsync(int tipoPessoaId);
        Task<TipoPessoa?> GetTipoPessoaByIdAsync(int id);
        Task<SubtipoPessoa?> GetSubtipoPessoaByIdAsync(int id);
        Task<TipoPessoa> CreateTipoPessoaAsync(TipoPessoa tipoPessoa);
        Task<SubtipoPessoa> CreateSubtipoPessoaAsync(SubtipoPessoa subtipoPessoa);
        Task UpdateTipoPessoaAsync(TipoPessoa tipoPessoa);
        Task UpdateSubtipoPessoaAsync(SubtipoPessoa subtipoPessoa);
        Task DeleteTipoPessoaAsync(int id);
        Task DeleteSubtipoPessoaAsync(int id);
    }
}
