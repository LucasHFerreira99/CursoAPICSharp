using APICatalogo.Data;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Models;
using APICatalogo.RateLimitOptions;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
}).AddJsonOptions(options =>
    options.JsonSerializerOptions
    .ReferenceHandler = ReferenceHandler.IgnoreCycles)
.AddNewtonsoftJson();


//var OrigensComAcessoPermitido = "_origensComAcessoPermitido";

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(OrigensComAcessoPermitido,
//    policy =>
//    {
//        policy.WithOrigins("https://apirequest.io", "https://meusite.com")
//        .WithMethods("GET", "POST")
//        .AllowAnyHeader()
//        .AllowCredentials();
//    });
//});



builder.Services.AddCors(options =>
{
    options.AddPolicy("OrigensComAcessoPermitido",
    policy =>
    {
        policy.WithOrigins("https://localhost:7022")
        .WithMethods("GET", "POST")
        .AllowAnyHeader();
    });
});


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    // c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });


    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "APICatalogo",
        Description = "Cat�logo de Produtos e Categorias",
        TermsOfService = new Uri("https://www.linkedin.com/in/lucasferreirah/"),
        Contact = new OpenApiContact
        {
            Name = "Lucas",
            Email = "lucashf99@outlook.com.br",
            Url = new Uri("https://www.linkedin.com/in/lucasferreirah/"),
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://www.linkedin.com/in/lucasferreirah/"),
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT ",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                         new string[] {}
                    }
                });
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();



string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection,
    ServerVersion.AutoDetect(mySqlConnection)));



var secretKey = builder.Configuration["JWT:SecretKey"]
                   ?? throw new ArgumentException("Invalid secret key!!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    options.AddPolicy("SuperAdminOnly", policy =>
                       policy.RequireRole("Admin").RequireClaim("id", "macoratti"));

    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    options.AddPolicy("ExclusiveOnly", policy =>
                      policy.RequireAssertion(context =>
                      context.User.HasClaim(claim =>
                                           claim.Type == "id" && claim.Value == "macoratti")
                                           || context.User.IsInRole("SuperAdmin")));
});

builder.Services.AddTransient<IMeuServico, MeuServico>();

//Desabilita a utiliza��o implicita do FromServicesW
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.DisableImplicitFromServicesParameters = true;
//});


var myOptions = new MyRateLimitOptions();

builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);



builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
    {
        options.PermitLimit = myOptions.PermitLimit;//1;
        options.Window = TimeSpan.FromSeconds(myOptions.Window);
        options.QueueLimit = myOptions.QueueLimit;//2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpcontext.User.Identity?.Name ?? httpcontext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 2,
                QueueLimit = 0,
                Window = TimeSpan.FromSeconds(10)
            }));
});

var temp = builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
                          new QueryStringApiVersionReader(),
                          new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));
builder.Services.AddScoped<ITokenService, TokenService>();



builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
            "APICatalogo");
    });
}

app.UseHttpsRedirection();

app.UseRateLimiter();

//app.UseCors(OrigensComAcessoPermitido);
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
