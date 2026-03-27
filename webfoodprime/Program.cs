using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using webfoodprime.Data;
using webfoodprime.Extensions;
using webfoodprime.Helpers;
using webfoodprime.Models;
using webfoodprime.Services.Implementations;
using webfoodprime.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//////////////////////////////////////////////////
// 🔷 DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//////////////////////////////////////////////////
// 🔷 Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//////////////////////////////////////////////////
// 🔷 Services
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddAppServices();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IFoodService, FoodService>();


//////////////////////////////////////////////////
// 🔷 JWT
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
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

//////////////////////////////////////////////////
// 🔷 Swagger + JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token dạng: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});



//////////////////////////////////////////////////
// 🔷 Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//////////////////////////////////////////////////
// 🔷 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//////////////////////////////////////////////////
// 🔷 Seed DB
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.Seed(services);
}

//////////////////////////////////////////////////
///
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class public so integration tests can reference it
public partial class Program { }
