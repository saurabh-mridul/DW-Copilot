using AzureDigitalTwinsPlatformAPIs;
using AzureDigitalTwinsPlatformAPIs.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.DigitalWorkplace.Integration.Extensions.DigitalTwins;
using Microsoft.DigitalWorkplace.Integration.Models.DigitalTwins;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Services
        .ConfigureServices()
        .ConfigureTokenCredential()
        .ConfigureClientExtensions();

// builder.Services.AddAuthentication().AddBearerToken();
// builder.Services.AddAuthorizationBuilder().AddPolicy("admin_access", policy =>
//                 policy.RequireRole("admin").RequireClaim("permission", "admin"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerGeneratorOptions>(options =>
{
    options.InferSecuritySchemes = true;
});

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

var clients = app.MapGroup("")
    .AddEndpointFilterFactory((handlerContext, next) =>
    {
        var loggerFactory = handlerContext.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("RequestAuditor");
        return invocationContext =>
        {
            logger.LogInformation("Received Request: {Method} {Path}", invocationContext.HttpContext.Request.Method, invocationContext.HttpContext.Request.Path);
            return next(invocationContext);
        };
    });


clients.MapGet("/building", async Task<Results<Ok<Building>, NotFound>> (ISpaceDigitalTwinService spaceService, string name) =>
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

clients.MapGet("/rooms", async Task<Results<Ok<IEnumerable<Room>>, NotFound>> (ISpaceDigitalTwinService spaceService, string buildingName, string floorName) =>
    TypedResults.Ok(await spaceService.GetRooms(buildingName, floorName)))
    .WithName("GetRooms")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Rooms by Building and Floor",
        Description = "Get information of rooms by building and floor from the Digital Integration Platform.",
        Tags = [new() { Name = "Digital Integration Platform" }],
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
        }
    });

clients.MapGet("/spaces", async (ISpaceDigitalTwinService spaceService) =>
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

clients.MapGet("/spaces/{id}", async Task<Results<Ok<Space>, NotFound>> (ISpaceDigitalTwinService spaceService, Guid id) =>
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

clients.MapGet("/sensors", async (ISensorDigitalTwinService sensorService) =>
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

clients.MapGet("/sensors/{id}", async Task<Results<Ok<Sensor>, NotFound>> (ISensorDigitalTwinService sensorService, Guid id) =>
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
