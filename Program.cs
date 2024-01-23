using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using web_api_praktikum;
using web_api_praktikum.Model;
using web_api_praktikum.Model.DTO.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "corsapp", policy => { policy.WithOrigins("*"); });
});

var dbConnStr = builder.Configuration.GetSection("DB:ConnStr").Get<string>();
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

// Add services to the container.
builder.Services.AddSingleton<DB>(new SqliteImpl(dbConnStr));
builder.Services.AddTransient<UserDTO, UserDTOSQLite>();
builder.Services.AddTransient<ProductDTO, ProductDTOSQLite>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey))
     };
 }
);
builder.Services.AddControllers().ConfigureApiBehaviorOptions
(options =>
     {
         options.InvalidModelStateResponseFactory = context =>
         {
             var result = new ValidationFailedResult(context.ModelState);
             result.ContentTypes.Add(MediaTypeNames.Application.Json);

             return result;
         };
     }
);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
