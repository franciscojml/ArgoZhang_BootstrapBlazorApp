// Services/DatabaseService.cs
using BootstrapBlazorApp2.Server.Data;
using BootstrapBlazorApp2.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BootstrapBlazorApp2.Server.Services;

public class DatabaseService : IDatabaseService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(AppDbContext context, ILogger<DatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TipoPessoa>> GetTiposPessoaAsync()
    {
        try
        {
            return await _context.TiposPessoa
                .Include(tp => tp.SubtiposPessoa)
                .OrderBy(tp => tp.Descricao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar tipos de pessoa");
            throw;
        }
    }

    public async Task<List<SubtipoPessoa>> GetSubtiposPessoaAsync()
    {
        try
        {
            return await _context.SubtiposPessoa
                .Include(sp => sp.TipoPessoa)
                .OrderBy(sp => sp.TipoPessoa.Descricao)
                .ThenBy(sp => sp.Descricao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar subtipos de pessoa");
            throw;
        }
    }

    public async Task<List<SubtipoPessoa>> GetSubtiposByTipoAsync(int tipoPessoaId)
    {
        try
        {
            return await _context.SubtiposPessoa
                .Where(sp => sp.TipoPessoaId == tipoPessoaId)
                .OrderBy(sp => sp.Descricao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar subtipos por tipo de pessoa {TipoPessoaId}", tipoPessoaId);
            throw;
        }
    }

    public async Task<TipoPessoa?> GetTipoPessoaByIdAsync(int id)
    {
        try
        {
            return await _context.TiposPessoa
                .Include(tp => tp.SubtiposPessoa)
                .FirstOrDefaultAsync(tp => tp.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar tipo de pessoa por ID {Id}", id);
            throw;
        }
    }

    public async Task<SubtipoPessoa?> GetSubtipoPessoaByIdAsync(int id)
    {
        try
        {
            return await _context.SubtiposPessoa
                .Include(sp => sp.TipoPessoa)
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar subtipo de pessoa por ID {Id}", id);
            throw;
        }
    }

    public async Task<TipoPessoa> CreateTipoPessoaAsync(TipoPessoa tipoPessoa)
    {
        try
        {
            _context.TiposPessoa.Add(tipoPessoa);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Tipo de pessoa criado: {Descricao}", tipoPessoa.Descricao);
            return tipoPessoa;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar tipo de pessoa");
            throw;
        }
    }

    public async Task<SubtipoPessoa> CreateSubtipoPessoaAsync(SubtipoPessoa subtipoPessoa)
    {
        try
        {
            _context.SubtiposPessoa.Add(subtipoPessoa);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Subtipo de pessoa criado: {Descricao}", subtipoPessoa.Descricao);
            return subtipoPessoa;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar subtipo de pessoa");
            throw;
        }
    }

    public async Task UpdateTipoPessoaAsync(TipoPessoa tipoPessoa)
    {
        try
        {
            _context.TiposPessoa.Update(tipoPessoa);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Tipo de pessoa atualizado: {Id} - {Descricao}", tipoPessoa.Id, tipoPessoa.Descricao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar tipo de pessoa {Id}", tipoPessoa.Id);
            throw;
        }
    }

    public async Task UpdateSubtipoPessoaAsync(SubtipoPessoa subtipoPessoa)
    {
        try
        {
            _context.SubtiposPessoa.Update(subtipoPessoa);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Subtipo de pessoa atualizado: {Id} - {Descricao}", subtipoPessoa.Id, subtipoPessoa.Descricao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar subtipo de pessoa {Id}", subtipoPessoa.Id);
            throw;
        }
    }

    public async Task DeleteTipoPessoaAsync(int id)
    {
        try
        {
            var tipoPessoa = await _context.TiposPessoa.FindAsync(id);
            if (tipoPessoa != null)
            {
                _context.TiposPessoa.Remove(tipoPessoa);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tipo de pessoa removido: {Id}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover tipo de pessoa {Id}", id);
            throw;
        }
    }

    public async Task DeleteSubtipoPessoaAsync(int id)
    {
        try
        {
            var subtipoPessoa = await _context.SubtiposPessoa.FindAsync(id);
            if (subtipoPessoa != null)
            {
                _context.SubtiposPessoa.Remove(subtipoPessoa);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Subtipo de pessoa removido: {Id}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover subtipo de pessoa {Id}", id);
            throw;
        }
    }
}
