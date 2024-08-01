using APICatalogo.Models;

namespace APICatalogo.Repositories;

public interface IUnitOfWork
{
    //IRepository<Produto> ProdutoRepository { get; }
    //IRepository<Categoria> CategoriaRepository { get; }
    IProdutoRepository ProdutoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    void Commit();
}
