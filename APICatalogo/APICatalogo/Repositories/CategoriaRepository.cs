using APICatalogo.Data;
using APICatalogo.Models;
using System.Linq.Expressions;

namespace APICatalogo.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }
}
