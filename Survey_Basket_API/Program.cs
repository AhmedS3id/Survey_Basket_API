using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
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

builder.Services.AddHybridCache();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization = [
        new HangfireCustomBasicAuthenticationFilter{
            User=app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass=app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
        ],
    DashboardTitle = "Survey Basket Dashboard",
   // IsReadOnlyFunc = (DashboardContext context) => true

});

//var ScopFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
//using var scope = ScopFactory.CreateScope();
//var NotificationService = scope.ServiceProvider.GetService<INotificationService>();

//RecurringJob.AddOrUpdate("NotificationService", () => NotificationService!.SendNewPollsNotification(null), Cron.Daily);

RecurringJob.AddOrUpdate<INotificationService>(
    "NotificationService",
    service => service.SendNewPollsNotification(null),
    Cron.Daily
);

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.UseRateLimiter();

app.MapHealthChecks("health",new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
