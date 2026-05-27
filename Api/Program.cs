using Api.Core.Modules.Auth.Application.Interfaces;
using Api.Core.Modules.Auth.Application.UseCases;
using Api.Core.Modules.Auth.Infrastructure.Persistence;
using Api.Core.Modules.Auth.Infrastructure.Presentation;
using Api.Core.Modules.Auth.Infrastructure.Security;
using Api.Core.Modules.Products.Application.Interfaces;
using Api.Core.Modules.Products.Application.UseCases;
using Api.Core.Modules.Products.Infrastructure.Persistence;
using Api.Core.Modules.Products.Infrastructure.Presentation;
using Api.Core.Modules.PersonTypes.Application.Interfaces;
using Api.Core.Modules.PersonTypes.Application.UseCases;
using Api.Core.Modules.PersonTypes.Infrastructure.Persistence;
using Api.Core.Modules.Categories.Application.Interfaces;
using Api.Core.Modules.Categories.Application.UseCases;
using Api.Core.Modules.Categories.Infrastructure.Presentation;
using Api.Core.Modules.Categories.Infrastructure.Persistence;
using Api.Core.Modules.Persons.Application.Interfaces;
using Api.Core.Modules.Persons.Application.UseCases;
using Api.Core.Modules.Persons.Infrastructure.Presentation;
using Api.Core.Modules.Persons.Infrastructure.Persistence;
using Api.Core.Modules.Sales.Application.Interfaces;
using Api.Core.Modules.Sales.Application.UseCases;
using Api.Core.Modules.Sales.Infrastructure.Persistence;
using Api.Core.Modules.Sales.Infrastructure.Presentation;
using Api.Core.Modules.SaleDetails.Application.Interfaces;
using Api.Core.Modules.SaleDetails.Application.UseCases;
using Api.Core.Modules.SaleDetails.Infrastructure.Persistence;
using Api.Core.Modules.SaleParticipants.Application.Interfaces;
using Api.Core.Modules.SaleParticipants.Application.UseCases;
using Api.Core.Modules.SaleParticipants.Infrastructure.Persistence;
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

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod
        | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath
        | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestBody
        | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
    o.RequestBodyLogLimit = 4096;
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RegisterUseCase>();
builder.Services.AddScoped<IPasswordHasher, Rfc2898PasswordHasher>();

// Product CRUD
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<GetAllProductsUseCase>();
builder.Services.AddScoped<GetProductByIdUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<DeleteProductUseCase>();

// PersonType CRUD
builder.Services.AddScoped<IPersonTypeRepository, PersonTypeRepository>();
builder.Services.AddScoped<GetAllPersonTypesUseCase>();
builder.Services.AddScoped<GetPersonTypeByIdUseCase>();
builder.Services.AddScoped<CreatePersonTypeUseCase>();
builder.Services.AddScoped<UpdatePersonTypeUseCase>();
builder.Services.AddScoped<DeletePersonTypeUseCase>();

// Category CRUD
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<GetAllCategoriesUseCase>();
builder.Services.AddScoped<GetCategoryByIdUseCase>();
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<UpdateCategoryUseCase>();
builder.Services.AddScoped<DeleteCategoryUseCase>();

// Person CRUD
builder.Services.AddScoped<IPersonCrudRepository, PersonCrudRepository>();

// Sale CRUD
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<GetAllSalesUseCase>();
builder.Services.AddScoped<GetSaleByIdUseCase>();
builder.Services.AddScoped<CreateSaleUseCase>();
builder.Services.AddScoped<UpdateSaleUseCase>();
builder.Services.AddScoped<DeleteSaleUseCase>();

// SaleDetail CRUD
builder.Services.AddScoped<ISaleDetailRepository, SaleDetailRepository>();
builder.Services.AddScoped<GetAllSaleDetailsUseCase>();
builder.Services.AddScoped<GetSaleDetailByIdUseCase>();
builder.Services.AddScoped<CreateSaleDetailUseCase>();
builder.Services.AddScoped<UpdateSaleDetailUseCase>();
builder.Services.AddScoped<DeleteSaleDetailUseCase>();

// SaleParticipant CRUD
builder.Services.AddScoped<ISaleParticipantRepository, SaleParticipantRepository>();
builder.Services.AddScoped<GetAllSaleParticipantsUseCase>();
builder.Services.AddScoped<GetSaleParticipantByIdUseCase>();
builder.Services.AddScoped<CreateSaleParticipantUseCase>();
builder.Services.AddScoped<UpdateSaleParticipantUseCase>();
builder.Services.AddScoped<DeleteSaleParticipantUseCase>();
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
    options.AddPolicy("CatalogWrite", policy => policy.RequireRole("Administrador", "Vendedor"));
    options.AddPolicy("PeopleWrite", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("SalesWrite", policy => policy.RequireRole("Administrador", "Vendedor"));
});

var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var db      = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher  = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db, hasher);
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapPersonEndpoints();
app.MapProductEndpoints();
app.MapSaleEndpoints();
app.MapControllers();
app.Run();

public partial class Program { }
