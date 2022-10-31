using GroceryAPI.DAL;
using GroceryAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGroceryRepository, GroceryRepository>();
builder.Services.AddDbContext<GroceryDbContext>(o => o.UseInMemoryDatabase("MyGroceryDb"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = builder.Environment.ApplicationName,
        Version = "v1",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(GetDefaultOpenApiSecurityRequirement());
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// GET all grocery items
app.MapGet("/api/groceries", (IGroceryRepository groceryRepository) =>
{
    return TypedResults.Ok(groceryRepository.GetItems());
})
.WithName("GetGroceryItems")
.WithOpenApi();

// GET a grocery item by ID
app.MapGet("/api/groceries/{id:int}", Results<Ok<GroceryItem>, NotFound> ([FromRoute] int id, IGroceryRepository groceryRepository) =>
{
    var item = groceryRepository.GetItem(id);
    return item != null ? TypedResults.Ok(item) : TypedResults.NotFound();
})
.WithName("GetGroceryItem")
.WithOpenApi();

// POST a new grocery item
app.MapPost("/api/groceries", [Authorize] (IGroceryRepository groceryRepository, GroceryItem groceryItem) =>
{
    groceryRepository.AddItem(groceryItem);
    groceryRepository.SaveChanges();

    return TypedResults.Created($"/api/groceries/{groceryItem.Id}", groceryItem);
})
.WithName("AddGroceryItem")
.WithOpenApi(operation =>
{
    operation.Security = new List<OpenApiSecurityRequirement>
    {
       GetDefaultOpenApiSecurityRequirement()
    };

    return operation;
});


// PUT an existing grocery item
app.MapPut("/api/groceries/{id:int}", [Authorize] Results<NoContent, NotFound> ([FromRoute] int id, GroceryItem groceryItem, IGroceryRepository groceryRepository) =>
{
    var existingItem = groceryRepository.GetItem(id);
    
    if (existingItem is null)
    {
        return TypedResults.NotFound();
    }

    existingItem.Name = groceryItem.Name;
    existingItem.Description = groceryItem.Description;

    groceryRepository.UpdateItem(existingItem);
    groceryRepository.SaveChanges();

    return TypedResults.NoContent();
})
.WithName("UpdateGroceryItem")
.WithOpenApi(operation =>
{
    operation.Security = new List<OpenApiSecurityRequirement>
    {
       GetDefaultOpenApiSecurityRequirement()
    };

    return operation;
});


// DELETE a grocery item
app.MapDelete("/api/groceries/{id:int}", [Authorize] Results<NoContent, NotFound> ([FromRoute] int id, IGroceryRepository groceryRepository) =>
{
    var existingItem = groceryRepository.GetItem(id);

    if (existingItem is null)
    {
        return TypedResults.NotFound();
    }

    groceryRepository.RemoveItem(existingItem);
    groceryRepository.SaveChanges();

    return TypedResults.NoContent();
})
.WithName("DeleteGroceryItem")
.WithOpenApi(operation =>
{
    operation.Security = new List<OpenApiSecurityRequirement>
    {
       GetDefaultOpenApiSecurityRequirement()
    };

    return operation;
});

app.Run();

static OpenApiSecurityRequirement GetDefaultOpenApiSecurityRequirement() => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            };
