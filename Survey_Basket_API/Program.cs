using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Survey_Basket_API;
using Survey_Basket_API.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context,configurations)=>
{
    configurations.ReadFrom.Configuration(context.Configuration);
    //configurations
    //.MinimumLevel.Information()
    //.WriteTo.Console();
});

// Add services to the container.

builder.Services.Add_Dependencies(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
