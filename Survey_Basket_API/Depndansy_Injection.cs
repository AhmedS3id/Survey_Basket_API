using FluentValidation.AspNetCore;
using MapsterMapper;
using System.Reflection;

namespace Survey_Basket_API
{
    public static class Depndansy_Injection
    {
        public static IServiceCollection AddDependaces (this IServiceCollection services)
        {
           services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
           services.AddOpenApi();
           services.AddScoped<IPollServices, PollServices>();
            // Add Mapster 
            var Mapping_Conf = TypeAdapterConfig.GlobalSettings;
            Mapping_Conf.Scan(Assembly.GetExecutingAssembly());

           services.AddSingleton<IMapper>(new Mapper(Mapping_Conf));

           services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
           services.AddFluentValidationAutoValidation();
            return services;

        }

    }
}
