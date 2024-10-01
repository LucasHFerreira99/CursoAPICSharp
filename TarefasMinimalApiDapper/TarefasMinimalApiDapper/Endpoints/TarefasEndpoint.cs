using Dapper.Contrib.Extensions;
using System.Runtime.CompilerServices;
using TarefasMinimalApiDapper.Data;
using static TarefasMinimalApiDapper.Data.TarefaContext;

namespace TarefasMinimalApiDapper.Endpoints
{
    public static class TarefasEndpoint
    {
        public static void MapTarefasEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => $"Bem-vindo a TarefasMinimalAPIDapper - {DateTime.Now}");

            app.MapGet("/tarefas", async (GetConnection connectionGetter) =>
            {
                using var con = await connectionGetter();
                var tarefas = con.GetAll<Tarefa>().ToList();
                if (tarefas is null) return Results.NotFound();
                return Results.Ok(tarefas);
            });

            app.MapGet("/tarefas/{id}", async (GetConnection connectionGetter, int id) =>
            {
                using var con = await connectionGetter();
                return con.Get<Tarefa>(id) is Tarefa tarefa ? Results.Ok(tarefa) : Results.NotFound();
            });

            app.MapPost("/tarefas", async (GetConnection connectionGetter, Tarefa tarefa) =>
            {
                using var con = await connectionGetter();
                var id = con.Insert(tarefa);
                return Results.Created($"tarefas/{id}", tarefa);
            });

            app.MapPut("/tarefas", async (GetConnection connectionGetter, Tarefa tarefa) =>
            {
                using var con = await connectionGetter();
                var id = con.Update(tarefa);
                return Results.Ok();
            });

            app.MapDelete("/tarefas/{id}", async (GetConnection connectionGetter, int id) =>
            {
                using var con = await connectionGetter();

                var deleted = con.Get<Tarefa>(id);

                if (deleted is null) return Results.NotFound();

                con.Delete(deleted);
                return Results.Ok();
            });
        }
    }
}
