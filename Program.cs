using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Skillest.API.Application;
using SkillSet.API.Data;
using SkillSet.API.Repositories;

var builder = WebApplication.CreateBuilder(args);
const string sapCors = "corsapp";
// Add services to the container
builder.Services.AddDbContext<Context>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IOwnerApplication, OwnerApplication>();

builder.Services.AddControllers();

builder.Services.AddCors(p => p.AddPolicy(sapCors, builder =>
    {
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }));
    
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

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = new PathString("/Resources")
});

// app.UseRouting();
app.UseCors(sapCors);
app.UseAuthorization();

app.MapControllers();

app.Run();
