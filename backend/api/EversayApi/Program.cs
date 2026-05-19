using EversayApi.Data;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Modules.Auth;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using System.Text;
using SasTokenLib;
using SasTokenLib.SasTokenLib;
using Modules.EventImage;
using User_lib;
using EversayApi.Services;
using Event_lib.Repositories;

var builder = WebApplication.CreateBuilder(args);
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = "mongodb://localhost:27017",
        DatabaseName = "userdb",
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;
    }
};

builder.Services.ConfigureMongoDbIdentity<Applicationuser, ApplicationRole, Guid>(mongoDbIdentityConfig)
    .AddUserManager<UserManager<Applicationuser>>()
    .AddSignInManager<SignInManager<Applicationuser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(
        builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT key is not configured.")
    )
),
        ClockSkew = TimeSpan.Zero,
    };
    x.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var error = context.Error ?? "invalid_token";
            var errorDescription = context.ErrorDescription;

            if (string.IsNullOrEmpty(errorDescription))
            {
                errorDescription = "The token is invalid or has expired";
                if (context.AuthenticateFailure != null)
                {
                    if (context.AuthenticateFailure is SecurityTokenExpiredException expiredException)
                    {
                        errorDescription = $"The token expired on {expiredException.Expires:o}";
                    }
                    else
                    {
                        errorDescription = "Authentication failed: " + context.AuthenticateFailure.Message;
                    }
                }
            }

            var response = new { error, error_description = errorDescription };
            return context.Response.WriteAsJsonAsync(response);
        }

    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll", policy =>
    {
          policy.WithOrigins("http://localhost:9000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


//SasToken
builder.Services.AddSingleton<SasTokenRepository>();
builder.Services.AddSingleton<EventImageRepository>(sp =>
{
    var mongoService = sp.GetRequiredService<MongoDbService>();
    return new EventImageRepository(mongoService.Database);
});
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<EversayApi.Services.UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
