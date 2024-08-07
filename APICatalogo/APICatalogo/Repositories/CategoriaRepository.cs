using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Linq.Expressions;
using X.PagedList;

namespace APICatalogo.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters)
    {
        var categorias = await GetAllAsync();

        var categoriasOrdernadas = categorias.OrderBy(p => p.CategoriaId).AsQueryable();

        //var resultado = IPagedList<Categoria>.ToPagedList(categoriasOrdernadas, categoriasParameters.PageNumber, categoriasParameters.PageSize);
        var resultado = await categoriasOrdernadas.ToPagedListAsync(categoriasParameters.PageNumber, categoriasParameters.PageSize);

        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasParams)
    {
        var categorias = await GetAllAsync();
        

        if (!string.IsNullOrEmpty(categoriasParams.Nome))
        {
            categorias = categorias.Where(c => c.Nome.IndexOf(categoriasParams.Nome, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //var categoriasFiltradas = IPagedList<Categoria>.ToPagedList(categorias.AsQueryable(), categoriasParams.PageNumber, categoriasParams.PageSize);

        var categoriasFiltradas =  await categorias.ToPagedListAsync(categoriasParams.PageNumber, categoriasParams.PageSize);

        return categoriasFiltradas;
    }
}
