using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using GAWeb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(); // Enable serving static files

// Define the API endpoints
var experiments = new Dictionary<Guid, Experiment>();
var experimentIdCounter = 0;

app.MapPut("/api/experiments", async (ExperimentParameters parameters) =>
{
    lock (ExperimentStore._lock)
    {
        var id = Guid.NewGuid();
        Experiment experiment = ExperimentStore.NewExperiment(id, parameters);
        Console.WriteLine($"Created experiment with Matrix {JSONConverter.MatrixToJson(experiment.Matrix)}");
        return Results.Ok(
            new
            {
                ExperimentId = id,
                Matrix = JSONConverter.MatrixToJson(experiment.Matrix),
                Best = experiment.Best
            }
        );
    }
});

app.MapPost("/api/experiments/{id:guid}", async (Guid id, ExperimentParameters parameters) =>
{
    lock (ExperimentStore._lock)
    {
        Experiment experiment;
        if (!ExperimentStore.GetExperiments(id, out experiment))
        {
            experiment = ExperimentStore.NewExperiment(id, parameters);
        }

        var result = experiment.RunStep();
        return Results.Ok(result);
    }
});

app.MapDelete("/api/experiments/{id:guid}", (Guid id) =>
{
    lock (ExperimentStore._lock)
    {
        if (ExperimentStore.Experiments.Remove(id))
            return Results.Ok("Experiment deleted");

        return Results.NotFound("Experiment not found");
    }
});

app.MapPost("/api/experiments/{id:guid}/stop", (Guid id) =>
{
    lock (ExperimentStore._lock)
    {
        if (!ExperimentStore.EvolutionTokens.TryGetValue(id, out var cts))
            return Results.BadRequest("No running evolution for this experiment.");

        cts.Cancel();
        return Results.Ok("Evolution stopped.");
    }
});


app.MapFallbackToFile("index.html");

app.Run();

public record ExperimentParameters(
    int countPopulation,
    int countCity,
    int lr,
    int countThreads
);
