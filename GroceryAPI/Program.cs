using GroceryAPI.DAL;
using GroceryAPI.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGroceryRepository, GroceryRepository>();
builder.Services.AddDbContext<GroceryDbContext>(o => o.UseInMemoryDatabase("MyGroceryDb"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.MapPost("/api/groceries", (IGroceryRepository groceryRepository, GroceryItem groceryItem) =>
{
    groceryRepository.AddItem(groceryItem);
    groceryRepository.SaveChanges();

    return TypedResults.Created($"/api/groceries/{groceryItem.Id}", groceryItem);
})
.WithName("AddGroceryItem")
.WithOpenApi();

// PUT an existing grocery item
app.MapPut("/api/groceries/{id:int}", Results<NoContent, NotFound> ([FromRoute] int id, GroceryItem groceryItem, IGroceryRepository groceryRepository) =>
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
.WithOpenApi();


// DELETE a grocery item
app.MapDelete("/api/groceries/{id:int}", Results<NoContent, NotFound> ([FromRoute] int id, IGroceryRepository groceryRepository) =>
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
.WithOpenApi();

app.Run();
