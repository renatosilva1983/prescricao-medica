// Composition Root da API. Nesta etapa, só a estrutura: a API sobe e responde ao /health.
// Os serviços de cada camada entram nas próximas etapas (AddApplication, AddInfrastructure).

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
