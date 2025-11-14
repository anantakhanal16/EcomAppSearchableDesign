using Application.Dtos;
using Infrastructure.Jwt;
using Infrastructure.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddAppIdentity();
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddApplicationServices();
builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddSwaggerDocs(); 

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Swagger in dev only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcomWebapi API v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.MapControllers();
app.Run();
