using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using TicketSistemi.Context;
using TicketSistemi.Models;
using TicketSistemi.Services.Abstractions;
using TicketSistemi.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    string con = builder.Configuration.GetConnectionString("SqlServer")!;
    opt.UseSqlServer(con);    
});
builder.Services.ConfigureHttpJsonOptions(opt =>
{
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddIdentityCore<ApplicationUser>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequiredLength = 6;
    opt.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddCors();



var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();  //SSL helping for live
app.UseStaticFiles();    
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .SetPreflightMaxAge(TimeSpan.FromMinutes(10))); //caching cash 


app.UseAuthentication();
app.UseAuthorization();
app.Run();
