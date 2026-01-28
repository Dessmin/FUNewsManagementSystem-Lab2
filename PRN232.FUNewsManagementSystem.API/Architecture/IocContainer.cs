using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Services;
using System.Text;

public static class IocContainer
{
    public static IServiceCollection SetupIocContainer(this IServiceCollection services)
    {
        var configuration = GetConfiguration();

        // Add DbContext
        services.SetupDbContext(configuration);

        // Add Swagger
        services.SetupSwagger();

        // Add generic repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add HttpContextAccessor
        services.AddHttpContextAccessor();

        // Add business services
        services.SetupBusinessServicesLayer();

        // Add JWT Authentication
        services.SetupJwt(configuration);

        return services;
    }

    private static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static IServiceCollection SetupDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        }

        services.AddDbContext<NewsManagementDbContext>(options =>
            options.UseSqlServer(connectionString,
                sql => sql.MigrationsAssembly(typeof(NewsManagementDbContext).Assembly.FullName)
            )
        );

        return services;
    }

    public static IServiceCollection SetupBusinessServicesLayer(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        return services;
    }

    private static IServiceCollection SetupSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.UseInlineDefinitionsForEnums();

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FU News Management System API",
                Version = "v1",
                Description = @"API for managing news articles, categories, tags, and user accounts in FU News Management System.",
                Contact = new OpenApiContact
                {
                    Name = "FU News Team",
                    Email = "support@funews.edu.vn"
                }
            });

            // JWT Authentication configuration for Swagger
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter 'Bearer' [space] and then your valid JWT token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
            };

            c.AddSecurityRequirement(securityRequirement);

            c.UseAllOfForInheritance();
            c.EnableAnnotations();
        });

        return services;
    }

    private static IServiceCollection SetupJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JWT:SecretKey"];
        var issuer = configuration["JWT:Issuer"];
        var audience = configuration["JWT:Audience"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT:SecretKey not found in appsettings.json");
        }

        if (string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException("JWT:Issuer not found in appsettings.json");
        }

        if (string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT:Audience not found in appsettings.json");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Set to true in production with HTTPS
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero // Remove delay of token when expire
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("StudentPolicy", policy =>
                policy.RequireRole("Student"));

            options.AddPolicy("StaffPolicy", policy =>
                policy.RequireRole("Staff"));

            options.AddPolicy("LecturerPolicy", policy =>
                policy.RequireRole("Lecturer"));
        });

        return services;
    }
}

