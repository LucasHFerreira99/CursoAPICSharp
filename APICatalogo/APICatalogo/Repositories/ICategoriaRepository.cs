﻿using APICatalogo.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace APICatalogo.Repositories;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetCategorias();
    Categoria GetCetegoria(int id);
    Categoria Create (Categoria categoria);
    Categoria Update(Categoria categoria);
    Categoria Delete(int id);
}
