using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _repository;

        public ProdutosController(IProdutoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _repository.GetProdutos();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }
            return Ok(produtos);
        }

        [HttpGet("{id:int}", Name ="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {

            //throw new Exception("Exceção ao retornar o produto pelo ID");
            var produto = _repository.GetProduto(id);
            if(produto == null)
            {
                return NotFound("Produto não encontrado...");
            }
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if(produto is null)
            {
                return BadRequest();
            }
            var produtoCriado = _repository.Create(produto);

            return new CreatedAtRouteResult("ObterProduto", 
                new { id = produtoCriado.ProdutoId }, produtoCriado);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest();
            }
            var produtoAlterado = _repository.Update(produto);

            return Ok(produtoAlterado);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produtoDeletado = _repository.Delete(id);
            //var produto = _context.Produtos.Find(id);
            if(produtoDeletado is null)
            {
                return NotFound("Produto não localizado...");
            }

            return Ok(produtoDeletado);

        }
    }
}
