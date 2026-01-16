using FluentValidation.AspNetCore;
using MapsterMapper;
using Survey_Basket_API.Persistence;
using System.Reflection;

namespace Survey_Basket_API
{
    public static class Dependency_Injection
    {
        public static IServiceCollection Add_Dependencies (this IServiceCollection Services,
            IConfiguration Configuration)
        {
           Services.AddControllers();

            var ConnectionString = Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection String 'DefaultConnection' Is Not Found .");

              Services.AddDbContext<AppDbContext>
                (options => options.UseSqlServer(ConnectionString));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            Services.AddOpenApi();
           Services.AddScoped<IPollServices, PollServices>();
            // Add Mapster 
            var Mapping_Conf = TypeAdapterConfig.GlobalSettings;
            Mapping_Conf.Scan(Assembly.GetExecutingAssembly());

           Services.AddSingleton<IMapper>(new Mapper(Mapping_Conf));

           Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
           Services.AddFluentValidationAutoValidation();
            return Services;

        }

    }
}
