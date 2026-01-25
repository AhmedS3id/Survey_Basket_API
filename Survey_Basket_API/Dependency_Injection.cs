using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Survey_Basket_API.Authentication;
using Survey_Basket_API.Persistence;
using System.Reflection;
using System.Text;

namespace Survey_Basket_API
{
    public static class Dependency_Injection
    {
        public static IServiceCollection Add_Dependencies(this IServiceCollection Services,
            IConfiguration Configuration)
        {
            Services.AddControllers();
            Services.AddMapsterServicesConfig()
                .AddAuthConfig( Configuration);

         var ConnectionString = Configuration.GetConnectionString("DefaultConnection") ??
         throw new InvalidOperationException("Connection String 'DefaultConnection' Is Not Found .");

            Services.AddDbContext<AppDbContext>
              (options => options.UseSqlServer(ConnectionString));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            Services.AddOpenApi();

            //injection
            Services.AddScoped<IPollServices, PollServices>();
            Services.AddScoped<IAuthServices, AuthServices>();


            Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            Services.AddFluentValidationAutoValidation();
            return Services;

        }
        public static IServiceCollection AddMapsterServicesConfig(this IServiceCollection Services)
        {
            // Add Mapster 
            var Mapping_Conf = TypeAdapterConfig.GlobalSettings;
            Mapping_Conf.Scan(Assembly.GetExecutingAssembly());

            Services.AddSingleton<IMapper>(new Mapper(Mapping_Conf));
            return Services;
        }
        public static IServiceCollection AddAuthConfig(this IServiceCollection services,
            IConfiguration Configuration)
        {
            // services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));
            services.AddOptions<JwtOptions>()
                 .BindConfiguration("Jwt")
                 .ValidateDataAnnotations()
                 .ValidateOnStart();
            var JwtSettings = Configuration.GetSection("Jwt").Get<JwtOptions>();

            services.AddSingleton<IJwtProvider, JwtProvider>();   
            services.AddIdentity<ApplicationUser, IdentityRole>()
                  .AddEntityFrameworkStores<AppDbContext>();
            services.AddAuthentication(static option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings?.Key!)),
                    ValidIssuer = JwtSettings?.Issuer,
                    ValidAudience = JwtSettings?.Audience
                };
            }
            );
            var test = new
            {
                IssuerSigningKey = Configuration["Jwt:Key"]!,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"]
            };

            
            return services;
        }


    }
}
