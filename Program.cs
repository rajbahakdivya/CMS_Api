using CMS_Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CMS_Api.Data;
using CMS_Api.AutoMapperConfig;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var Key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// Add CORS policy to allow all origins, methods, and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Add Swagger with JWT Authorization support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGci...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Register AutoMapper, telling it to scan the assembly where ClientProfile lives:
//builder.Services.AddAutoMapper(typeof(ClientProfile));
builder.Services.AddAutoMapper(typeof(ClientProfile).Assembly);


// Register EF Core DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<AuthService>();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        ValidateLifetime = true,   // Ensure the token's `exp` claim is valid
        ClockSkew = TimeSpan.Zero, // Avoid issues with clock drift during validation

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully.");
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var decodeToken = new JwtSecurityToken(jwtEncodedString: token);
                string tenantId = decodeToken.Claims.First(c => c.Type == "tenantId").Value;
                //string userId = decodeToken.Claims.First(c => c.Type == "userId").Value;
                //string sessionId = decodeToken.Claims.First(c => c.Type == "sessionId").Value;

                context.Request.Headers["Tenant-ID"] = tenantId;
                //context.Request.Headers["User-ID"] = userId;
                //context.Request.Headers["Session-ID"] = sessionId;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine("Challenge issued: " + context.ErrorDescription);
            return Task.CompletedTask;
        }
    };
});


builder.WebHost.UseWebRoot("wwwroot");

var app = builder.Build();

// Use CORS before other middlewares
app.UseCors("AllowAll");

// Enable Swagger UI only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use HTTPS redirection only in non-development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();

app.Run();