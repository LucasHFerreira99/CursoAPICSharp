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
    public class PostProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {

        private readonly ProdutosController _controller;

        public PostProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task PostProduto_Return_CreatedStatusCode()
        {
            //Arrage
            var novoProdutoDto = new ProdutoDTO
            {
                Nome = "Novo Produto",
                Descricao = "Descricao do novo produto",
                Preco = 10.99m,
                ImagemUrl = "imagemfake1.jpg",
                CategoriaId = 2
            };

            //Act
            var data = await _controller.Post(novoProdutoDto);

            //Assert (fluentassertions
           var createadResult =  data.Result.Should().BeOfType<CreatedAtRouteResult>();
           createadResult.Subject.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task PostProduto_Return_BadRequest()
        {
            //Arrage
            ProdutoDTO novoProdutoDto = null;

            //Act
            var data = await _controller.Post(novoProdutoDto);

            //Assert 
            var createadResult = data.Result.Should().BeOfType<BadRequestResult>();
            createadResult.Subject.StatusCode.Should().Be(400);
        }
    }
}
