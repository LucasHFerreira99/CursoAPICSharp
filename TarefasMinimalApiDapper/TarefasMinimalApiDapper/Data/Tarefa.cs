﻿using System.ComponentModel.DataAnnotations.Schema;

namespace TarefasMinimalApiDapper.Data;

[Table("Tarefas")]
public record Tarefa (int Id, string Atividade, string Status);
