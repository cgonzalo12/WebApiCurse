using Biblioteca.Datos;
using Biblioteca.Entidades;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
//Area de servicios
builder.Services.AddDataProtection();

var origenesPermitidos = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;

builder.Services.AddCors(opcionres =>
{
    opcionres.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders("mi-cabezera");
    });
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddNewtonsoftJson(); 

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios,ServiciosUsuarios>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]!)),
        ClockSkew = TimeSpan.Zero
    };
});
//politicas
builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});

builder.Services.AddSwaggerGen();


var app = builder.Build();

// Area de middlewares
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapControllers();

app.Run();
