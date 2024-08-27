using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class PutProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;

        public PutProdutosUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task PutProduto_Update_Return_OkResult()
        {
            //Arrage
            var id = 17;
            var novoProdutoDto = new ProdutoDTO
            {
                ProdutoId = id,
                Nome = "Novo Produto",
                Descricao = "Descricao do novo produto",
                Preco = 10.99m,
                ImagemUrl = "imagemfake1.jpg",
                CategoriaId = 2
            };

            //Act
            var data = await _controller.Put(id, novoProdutoDto) as ActionResult<ProdutoDTO>;

            //Assert
            data.Should().NotBeNull();
            data.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutProduto_Update_Return_BadRequest()
        {
            //Arrage
            var id = 17;
            var novoProdutoDto = new ProdutoDTO
            {
                ProdutoId = 1,
                Nome = "Novo Produto",
                Descricao = "Descricao do novo produto",
                Preco = 10.99m,
                ImagemUrl = "imagemfake1.jpg",
                CategoriaId = 2
            };

            //Act
            var data = await _controller.Put(id, novoProdutoDto);

            //Assert
            data.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be(400);
        }
    }
}
