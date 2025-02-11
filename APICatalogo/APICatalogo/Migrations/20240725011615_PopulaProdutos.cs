﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Produtos(Nome,Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
                "values ('Coca-Cola Diet','Refrigerante de Cola 350ml', 5.45 ,'cocacola.jpg', 50, now(), 1)");
            
            mb.Sql("Insert into Produtos(Nome,Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
             "values ('Lanche de Salsicha','Lanche de Salsicha com maionese', 8.50 ,'salsicha.jpg', 10, now(), 2)");

            mb.Sql("Insert into Produtos(Nome,Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
                "values ('Pudim 100g','Pudim de leite condensado 100g', 6.75 ,'pudim.jpg', 20, now(), 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Produtos");
        }
    }
}
