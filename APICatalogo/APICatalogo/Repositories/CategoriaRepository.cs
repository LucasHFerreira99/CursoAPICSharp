using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Linq.Expressions;

namespace APICatalogo.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters)
    {
        var categorias = await GetAllAsync();

        var categoriasOrdernadas = categorias.OrderBy(p => p.CategoriaId).AsQueryable();
        
        var resultado = PagedList<Categoria>.ToPagedList(categoriasOrdernadas, categoriasParameters.PageNumber, categoriasParameters.PageSize);
        return resultado;
    }

    public async Task<PagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasParams)
    {
        var categorias = await GetAllAsync();
        

        if (!string.IsNullOrEmpty(categoriasParams.Nome))
        {
            categorias = categorias.Where(c => c.Nome.IndexOf(categoriasParams.Nome, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias.AsQueryable(), categoriasParams.PageNumber, categoriasParams.PageSize);
        return categoriasFiltradas;
    }
}
