﻿using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

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

        public async Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams)
        {
            var getProdutos = await GetAllAsync();

            var produtos = getProdutos.OrderBy(p => p.ProdutoId).AsQueryable();

            //var produtosOrdenados = PagedList<Produto>.ToPagedList(produtos, produtosParams.PageNumber, produtosParams.PageSize);
            var produtosOrdenados = await produtos.ToPagedListAsync(produtosParams.PageNumber, produtosParams.PageSize);
            
            return produtosOrdenados;
        }

        public async Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams)
        {
            var produtos = await GetAllAsync();

            if(produtosFiltroParams.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
            {
                if (produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p=> p.Preco > produtosFiltroParams.Preco.Value).OrderBy(p=> p.Preco);
                }
                if (produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco < produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
                }
                if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco == produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
                }
            }

            //var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos.AsQueryable(), produtosFiltroParams.PageNumber, produtosFiltroParams.PageSize);
            var produtosFiltrados = await produtos.ToPagedListAsync(produtosFiltroParams.PageNumber, produtosFiltroParams.PageSize);
            return produtosFiltrados;
        }

        public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
        {
            var produtos = await GetAllAsync();
            var produtosCategoria = produtos.Where(c => c.CategoriaId == id);
            return produtosCategoria;
        }
    }
}
