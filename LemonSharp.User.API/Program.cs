using LemonSharp.User.API.DataAccess;
using LemonSharp.User.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add framework services.
builder.Services.AddDbContext<UserIdentityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Identity")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<UserIdentityDbContext>()
    .AddDefaultTokenProviders();

// Add JWT token validation
var secretKey = builder.Configuration.GetAppSetting("SecretKey");
var signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secretKey));

var tokenAudience = builder.Configuration.GetAppSetting("TokenAudience");
var tokenIssuer = builder.Configuration.GetAppSetting("TokenIssuer");
builder.Services.Configure<SparkTodo.API.JWT.TokenOptions>(options =>
{
    options.Audience = tokenAudience;
    options.Issuer = tokenIssuer;
    options.ValidFor = TimeSpan.FromHours(2);
    options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // The signing key must match!
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            // Validate the JWT Issuer (iss) claim
            ValidateIssuer = true,
            ValidIssuer = tokenIssuer,
            // Validate the JWT Audience (aud) claim
            ValidateAudience = true,
            ValidAudience = tokenAudience,
            // Validate the token expiry
            ValidateLifetime = true,
            // If you want to allow a certain amount of clock drift, set that here:
            ClockSkew = System.TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
