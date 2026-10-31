using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TeamDesk.Api.Data;
using TeamDesk.Api.Entities;
using TeamDesk.Api.Middlewares;
using TeamDesk.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES (avant Build) =================

// Contrôleurs + enums envoyés en texte dans le JSON ("New" au lieu de 0)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

// Base de données
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services métier
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IAuthService, AuthService>();      // Scoped car il utilise le DbContext (Scoped)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>(); // sans état → Singleton

// Authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,            // refuse un token expiré
            ValidateIssuerSigningKey = true,    // vérifie la signature
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// CORS : seule l'application Angular (localhost:4200) peut appeler l'API depuis un navigateur
builder.Services.AddCors(options =>
    options.AddPolicy("AngularDev", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()                 // nécessaire pour l'en-tête Authorization (JWT)
              .AllowAnyMethod()));              // GET, POST, PUT, PATCH...

builder.Services.AddScoped<ILookupService, LookupService>();
// ================= PIPELINE (après Build) =================
var app = builder.Build();

if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();          // le DbContext est Scoped : il faut un scope
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();                                 // applique les migrations (et le seed HasData)
}

app.UseMiddleware<ExceptionMiddleware>();       // 1. en premier : attrape toutes les erreurs

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TeamDesk API");
    });
}

app.UseHttpsRedirection();
app.UseCors("AngularDev");                      // 2. avant l'auth : le preflight OPTIONS n'a pas de token
app.UseAuthentication();                        // 3. qui es-tu ?
app.UseAuthorization();                         // 4. as-tu le droit ?
app.MapControllers();

app.Run();