using APICatalogo.Data;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers;

[EnableCors("OrigensComAcessoPermitido")]
[Route("[controller]")]
[ApiController]
[Produces("application/json")]  
//[EnableRateLimiting("fixedwindow")]
public class CategoriasController : ControllerBase
{

    private readonly IUnitOfWork _uof;
    private readonly ILogger _logger;
    public CategoriasController(ILogger<CategoriasController> logger, IUnitOfWork uof)
    {
        _uof = uof;
        _logger = logger;
    }

    [HttpGet("UsandoFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico meuServico, string nome)
    {
        return meuServico.Saudacao(nome);
    }

    [HttpGet("SemUsarFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoSemFromServices(IMeuServico meuServico, string nome)
    {
        return meuServico.Saudacao(nome);
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriasDto = categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);
    }

    [HttpGet("Pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltroNomeAsync([FromQuery] CategoriasFiltroNome categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasParameters);
        return ObterCategorias(categorias);
    }


    /// <summary>
    /// Obtem uma lista de objetos Categoria
    /// </summary>
    /// <returns>Retorna uma lista de objetos Categoria</returns>
    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    //[Authorize(Policy="UserOnly")]
    [DisableRateLimiting]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {
        _logger.LogInformation("=================== GET api/categorias ===================");
        var categorias = await _uof.CategoriaRepository.GetAllAsync();

        if (categorias is null)
            return NotFound("Não existem categorias...");

        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }


    /// <summary>
    /// Obtem uma Categoria pelo seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>objeto Categoria</returns>
    [DisableCors]
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
        if (categoria == null)
        {
            _logger.LogInformation($"=================== GET api/categorias/id = {id} NOT FOUND  ===================");

            return NotFound($"Categoria com id={id} não encontrada...");
        }

        var categoriaDto = categoria.ToCategoriaDTO();

        return Ok(categoriaDto);
    }

    /// <summary>
    /// Inclui uma nova categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de Request:
    /// 
    /// 
    ///     Post api/categorias
    ///     {
    ///        "categoriaId: 1, 
    ///        "nome": "categoria1",
    ///        "imagemUrl: "http://teste.net/1.jpg"
    ///     }
    /// </remarks>
    /// <param name="categoriaDto">objeto Categoria</param>
    /// <returns>O objeto categoria incluido</returns>
    /// <remarks>Retorna um objeto categoria incluído</remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            return BadRequest();
        }
        var categoria = new Categoria()
        {
            CategoriaId = categoriaDto.CategoriaId,
            Nome = categoriaDto.Nome,
            ImagemUrl = categoriaDto.ImagemUrl
        };

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();

        var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning($"Datos Invalidos...");
            return BadRequest("Dados invalidos");
        }

        var categoria = categoriaDto.ToCategoria();

        var categoriaAtualizado = _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();

        var CategoriaAtualizadaDto = categoriaAtualizado.ToCategoriaDTO();

        return Ok(CategoriaAtualizadaDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada");
            return NotFound($"Categoria com id={id} não encontrada...");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        await _uof.CommitAsync();

        var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDto);
    }
}
