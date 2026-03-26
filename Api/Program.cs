using Api.Core.Modules.Auth.Application.Interfaces;
using Api.Core.Modules.Auth.Application.UseCases;
using Api.Core.Modules.Auth.Infrastructure.Persistence;
using Api.Core.Modules.Auth.Infrastructure.Security;
using Api.Core.Modules.Persons.Application.Interfaces;
using Api.Core.Modules.Persons.Application.UseCases;
using Api.Core.Modules.Persons.Infrastructure.Persistence;
using Api.Core.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RegisterUseCase>();
builder.Services.AddScoped<IPasswordHasher, Rfc2898PasswordHasher>();

// Person CRUD
builder.Services.AddScoped<IPersonCrudRepository, PersonCrudRepository>();
builder.Services.AddScoped<GetAllPersonsUseCase>();
builder.Services.AddScoped<GetPersonByIdUseCase>();
builder.Services.AddScoped<CreatePersonUseCase>();
builder.Services.AddScoped<UpdatePersonUseCase>();
builder.Services.AddScoped<DeletePersonUseCase>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });
});

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CatalogWrite", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("PeopleWrite", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SalesWrite", policy => policy.RequireRole("Admin", "Seller"));
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
