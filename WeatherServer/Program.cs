using CountryModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WeatherServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Pasted from Pronto, professor got from OpenApi Documentation
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new()
    {
        Contact = new()
        {
            Email = "luis.munoz.91@my.csun.edu",
            Name = "Luis Munoz",
            Url = new("https://canvas.csun.edu/courses/128137")
        },
        Description = "APIs for World Cities",
        Title = "World Cities APIs",
        Version = "V1"
    });
    OpenApiSecurityScheme jwtSecurityScheme = new()
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Please enter *only* JWT token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, [] }
    });
});







//WE NEED TO ADD DBCONTEXT SERVICE IN HERE
//We are adding IdentityContext to tables
//404 error, Security trumps architecture (method does exist but server doesn't let user know it exists, security in Microsoft does this, maybe to prevent hackers)

//Next is to build authentication. (OCTA instead of sql-Server).

builder.Services.AddDbContext<CountriesSilverContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<WorldCitiesUser, IdentityRole>()
    .AddEntityFrameworkStores<CountriesSilverContext>();

//When installing make sure it has COre in name

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            builder.Configuration["JwtSettings:SecurityKey"] ?? throw new InvalidOperationException()))
    };
});
//Injection, fixed
builder.Services.AddScoped<JwtHandler>();


//Three ways to inject, 
// Transient create new object everytime invoked service in controller (needs to small and fast)
// Scoped (default) easy to code and maintain : created once in a call, client to server. As long as object is created it is re-used. Not worry of global variables
// Singleton: Created once when first invoked and never destroyed. Ex1: Used in DBContext when making connection to db.
// Ex2: Log : outputs all the log Information keeping output consistent.
//We are going to implement scoped 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
