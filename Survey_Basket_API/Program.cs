using Survey_Basket_API;
using Survey_Basket_API.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Add_Dependencies(builder.Configuration);
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection String 'DefaultConnection' Is Not Found .");

builder.Services.AddDbContext<AppDbContext>
    (options=>options.UseSqlServer(ConnectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
