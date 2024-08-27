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
    public class DeleteProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;

        public DeleteProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task DeleteProdutoById_Return_OkResult()
        {
            //Arrage
            var prodId = 2;

            //Act
            var data = await _controller.Delete(prodId);

            //Assert
            data.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(200);
        }
        [Fact]
        public async Task DeleteProdutoById_Return_NotFound()
        {
            //Arrage
            var prodId = 999;

            //Act
            var data = await _controller.Delete(prodId) as ActionResult<ProdutoDTO>;

            //Assert
            data.Should().NotBeNull();
            data.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
