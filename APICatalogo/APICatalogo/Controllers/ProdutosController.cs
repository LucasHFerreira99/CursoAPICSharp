using APICatalogo.Data;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }


        [HttpGet("categoria/{id}")]
        public ActionResult <IEnumerable<ProdutoDTO>> GetProdutosCategoria(int id)
        {
            var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);
            if(produtos is null)
            {
                return NotFound();
            }
            // var destino = _mapper.Map<TipoDestino>(origem)

            var produtosDTo = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtosDTo);
        }

        [HttpGet("Pagination")]
        public ActionResult<IEnumerable<ProdutoDTO>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = _uof.ProdutoRepository.GetProdutos(produtosParameters);

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            var produtos = _uof.ProdutoRepository.GetAll();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }

            var produtosDTo = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtosDTo);
        }

        [HttpGet("{id:int}", Name ="ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {

            //throw new Exception("Exceção ao retornar o produto pelo ID");
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            if(produto == null)
            {
                return NotFound("Produto não encontrado...");
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDto);
        }

        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
        {
            if(produtoDto is null)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoCriado = _uof.ProdutoRepository.Create(produto);
            _uof.Commit();

            var produtoDtoCriado = _mapper.Map<ProdutoDTO>(produtoCriado);
            return new CreatedAtRouteResult("ObterProduto", 
                new { id = produtoDtoCriado.ProdutoId }, produtoDtoCriado);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProdutoDTOUpdateResponse> Patch (int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if(patchProdutoDTO is null || id <= 0)
            {
                return BadRequest();
            }

            var produto = _uof.ProdutoRepository.Get(c => c.ProdutoId == id);

            if (produto is null)
                return NotFound();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);
            if(!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
                return BadRequest(ModelState);

            _mapper.Map(produtoUpdateRequest, produto);
            _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
        {
            if(id != produtoDto.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAlterado = _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            var produtoAlteradoDto = _mapper.Map<ProdutoDTO>(produtoAlterado);
            return Ok(produtoAlteradoDto);
                
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {

            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            if (produto == null)
                return NotFound("Produto não encontrado...");

            var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();

            var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);
            return Ok(produtoDeletadoDto);

        }
    }
}
