using FruitShop.API.Configurations;
using FruitShop.Application.Catalogues;
using FruitShop.Application.Commands;
using FruitShop.Application.Interfaces;
using FruitShop.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CalculateOrderCommand).Assembly);
});
builder.Services.AddSingleton<IFruitCatalogue, InMemoryFruitCatalogue>();
builder.Services.AddScoped<IOrderCalculationService, OrderCalculationService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ApplySwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();