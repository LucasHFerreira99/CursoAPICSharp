using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        //public IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParams)
        //{
        //    return GetAll()
        //        .OrderBy(p => p.Nome)
        //        .Skip((produtosParams.PageNumber - 1) * produtosParams.PageSize)
        //        .Take(produtosParams.PageSize).ToList();
        //}

        public PagedList<Produto> GetProdutos(ProdutosParameters produtosParams)
        {
            var produtos = GetAll().OrderBy(p => p.ProdutoId).AsQueryable();
            var produtosOrdernados = PagedList<Produto>.ToPagedList(produtos, produtosParams.PageNumber, produtosParams.PageSize);
            return produtosOrdernados;
        }

        public IEnumerable<Produto> GetProdutosPorCategoria(int id)
        {
            return GetAll().Where(c => c.CategoriaId == id);
        }
    }
}
