using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OData;
using PrescricaoMedica.Api.ErrorHandling;
using PrescricaoMedica.Api.OData;
using PrescricaoMedica.Application;
using PrescricaoMedica.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Composition Root: cada camada registra o que é dela; a Api só monta as peças.
builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddOData(options => options
        .Select().Filter().OrderBy().Count().SetMaxTop(ODataModel.TopMaximo)
        .AddRouteComponents(ODataModel.Prefixo, ODataModel.Criar()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

/// <summary>Permite testes de integração com WebApplicationFactory.</summary>
public partial class Program;
