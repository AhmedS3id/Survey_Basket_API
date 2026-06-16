using Asp.Versioning;
using FluentValidation.AspNetCore;
using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Survey_Basket_API.Health;
using Survey_Basket_API.Persistence;
using Survey_Basket_API.Settings;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace Survey_Basket_API
{
    public static class Dependency_Injection
    {
        public static IServiceCollection Add_Dependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            var AllowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()!;

            services.AddCors(options => options.AddDefaultPolicy(builder => builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(AllowedOrigins)
            ));

            //Services.AddCors(options => options.AddPolicy("MyPolicy",builder=>
            //builder
            //.WithOrigins(AllowedOrigins)
            //.AllowAnyMethod()
            //.AllowAnyHeader()
            //));
            services.AddControllers();
            services.AddMapsterServicesConfig()
                .AddAuthConfig(configuration);

         var ConnectionString = configuration.GetConnectionString("DefaultConnection") ??
         throw new InvalidOperationException("Connection String 'DefaultConnection' Is Not Found .");

            services.AddDbContext<AppDbContext>
              (options => options.UseSqlServer(ConnectionString));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();

            //injection
            services.AddScoped<IPollServices, PollServices>();
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IQuestionServices, QuestionService>();
            services.AddScoped<IVoteServices, VoteServices>();
            services.AddScoped<IResultServices, ResultServices>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailSender, EmailServices>();
            services.AddScoped<IRoleServices, RoleServices>();

            services.AddBackgroundJobsConfig(configuration);

            services.AddHealthChecks()
                .AddSqlServer( ConnectionString)
                .AddHangfire(Options=>Options.MinimumAvailableServers=1)
                .AddCheck<MailProviderHealthCheck>(name:"mail services");

            services.AddRateLimiter(RLOption =>
            {
                RLOption.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                RLOption.AddPolicy("ipLimit", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey:httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory:_=>new FixedWindowRateLimiterOptions
                    {
                        PermitLimit=2,
                        Window=TimeSpan.FromSeconds(20)
                    }
                ));
                RLOption.AddPolicy("userLimit", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey:httpContext.User.Identity?.Name?.ToString(),
                    //partitionKey:httpContext.User.GetUserId(),
                    factory:_=>new FixedWindowRateLimiterOptions
                    {
                        PermitLimit=2,
                        Window=TimeSpan.FromSeconds(20)
                    }
                ));
                RLOption.AddConcurrencyLimiter("concurrency", option =>
                {
                    option.PermitLimit = 2;
                    option.QueueLimit = 1;
                    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
                //RLOption.AddTokenBucketLimiter("tokens", option =>
                //{
                //    option.TokenLimit = 10;
                //    option.QueueLimit = 5;
                //    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                //    option.ReplenishmentPeriod=TimeSpan.FromSeconds(10);
                //    option.TokensPerPeriod = 2;
                //    option.AutoReplenishment = true;
                //}
                //RLOption.AddFixedWindowLimiter("fixed", option =>
                //{
                //    option.PermitLimit = 2;
                //    option.QueueLimit = 1;
                //    option.Window = TimeSpan.FromSeconds(20);
                //    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                //});
                //RLOption.AddSlidingWindowLimiter("sliding", option =>
                //{
                //    option.PermitLimit = 2;
                //    option.QueueLimit = 1;
                //    option.Window = TimeSpan.FromSeconds(20);
                //    option.SegmentsPerWindow = 2;
                //    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                //});

            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

            services.AddApiVersioning(option =>
            {
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.ReportApiVersions = true;
                option.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            }).AddApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'V";
                option.SubstituteApiVersionInUrl = true;
            });

            return services;

        }
        private static IServiceCollection AddMapsterServicesConfig(this IServiceCollection Services)
        {
            // Add Mapster 
            var Mapping_Conf = TypeAdapterConfig.GlobalSettings;
            Mapping_Conf.Scan(Assembly.GetExecutingAssembly());

            Services.AddSingleton<IMapper>(new Mapper(Mapping_Conf));
            return Services;
        }
        private static IServiceCollection AddAuthConfig(this IServiceCollection services,
            IConfiguration Configuration)
        {
            // services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));
            services.AddOptions<JwtOptions>()
                 .BindConfiguration("Jwt")
                 .ValidateDataAnnotations()
                 .ValidateOnStart();
            var JwtSettings = Configuration.GetSection("Jwt").Get<JwtOptions>();

            services.AddSingleton<IJwtProvider, JwtProvider>();   

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddIdentity<ApplicationUser,ApplicationRole>()
                  .AddEntityFrameworkStores<AppDbContext>()
                  .AddDefaultTokenProviders();

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
            });
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });
            var test = new
            {
                IssuerSigningKey = Configuration["Jwt:Key"]!,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"]
            };

            
            return services;
        }
        private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add Hangfire services.
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
            return services;
        }

    }
}
