using AzureDigitalTwinsPlatformAPIs;
using AzureDigitalTwinsPlatformAPIs.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.DigitalWorkplace.Integration.Extensions.DigitalTwins;
using Microsoft.DigitalWorkplace.Integration.Models.DigitalTwins;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services
        .ConfigureServices()
        .ConfigureTokenCredential()
        .ConfigureClientExtensions();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// app.UseStatusCodePages(async statusCodeContext
//     => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
//         .ExecuteAsync(statusCodeContext.HttpContext));

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(options =>
 {
     options.PreSerializeFilters.Add((swagger, httpRequest) =>
     {
         swagger.Servers = [new() { Url = $"{httpRequest.Scheme}://{httpRequest.Host.Value}" }];
     });
 });
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();


app.MapGet("/building", async Task<Results<Ok<Building>, NotFound>> (ISpaceDigitalTwinService spaceService, string name) =>
    await spaceService.GetBuilding(name) is { } Building
        ? TypedResults.Ok(Building)
        : TypedResults.NotFound())
    .WithName("GetBuildingByName")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Building by given Name",
        Description = "Get information of a building it's name from the Digital Integration Platform.",
        Tags = [new() { Name = "Digital Integration Platform" }],
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
        }
    });

app.MapGet("/spaces", async (ISpaceDigitalTwinService spaceService) =>
    TypedResults.Ok(await spaceService.GetSpaces()))
    .WithName("GetSpaces")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Spaces information",
        Description = "Returns information about all the available space digital twins from the Digital Integration Platform.",
        Tags = [new() { Name = "Digital Integration Platform" }],
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
        }
    });

app.MapGet("/spaces/{id}", async Task<Results<Ok<Space>, NotFound>> (ISpaceDigitalTwinService spaceService, Guid id) =>
    await spaceService.GetSpace(id) is { } Space
        ? TypedResults.Ok(Space)
        : TypedResults.NotFound())
    .WithName("GetSpaceById")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get space by given Id",
        Description = "Get information of a space by it's id from the Digital Integration Platform.",
        Tags = [new() { Name = "Digital Integration Platform" }],
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
        }
    });

app.MapGet("/sensors", async (ISensorDigitalTwinService sensorService) =>
    TypedResults.Ok(await sensorService.GetSensors()))
    .WithName("GetSensors")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Sensors Digital Twins",
        Description = "Returns information about all the available sensor digital twins from the Digital Integration Platform.",
        Tags = [new() { Name = "Digital Integration Platform" }],
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
        }
    });

app.MapGet("/sensors/{id}", async Task<Results<Ok<Sensor>, NotFound>> (ISensorDigitalTwinService sensorService, Guid id) =>
    await sensorService.GetSensor(id) is { } Sensor
        ? TypedResults.Ok(Sensor)
        : TypedResults.NotFound())
    .WithName("GetSensorById")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get sensor by given Id",
        Description = "Get information of a sensor by it's id from from the Digital Integration Platform.",
        Tags = [new() { Name = "Digital Integration Platform" }],
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
        }
    });

app.Run();
