using GestionDeDevoluciones.Backend.Auth;
using GestionDeDevoluciones.Services;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =============================
// CORS
// =============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// =============================
// DATABASE
// =============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// =============================
// SERVICES
// =============================
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IRemitoService, RemitoService>();
builder.Services.AddScoped<IPersonalService, PersonalService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IDecisionAdoptadaService, DecisionAdoptadaService>();
builder.Services.AddScoped<ITipoEstadoService, TipoEstadoService>();
builder.Services.AddScoped<IObservacionService, ObservacionService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// =============================
// JWT AUTHENTICATION
// =============================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "superSecretKey1234567890123456")
            )
        };
    });

builder.Services.AddAuthorization();

// =============================
// CONTROLLERS + SWAGGER
// =============================
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =============================
// MIDDLEWARE PIPELINE
// =============================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); 

app.UseCors("AllowFrontend");

// 🔐 IMPORTANTE: este orden
app.UseAuthentication();
app.UseAuthorization();

// 🌱 SEEDING
try 
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        Console.WriteLine("🌱 Iniciando DbInitializer...");
        DbInitializer.Initialize(context);
        Console.WriteLine("✅ DbInitializer completado.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR EN SEEDING: {ex.Message}");
}

// Serve uploaded files
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.MapControllers();

Console.WriteLine("🚀 Iniciando aplicación en http://localhost:5299 ...");
app.Run();
