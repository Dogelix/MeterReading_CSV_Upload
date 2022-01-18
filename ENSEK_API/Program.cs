using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("ensek_energy");
builder.Services.AddDbContext<ENSEK_API.Data.EnsekEnergyContext>(x => x.UseSqlServer(connectionString));

var AllowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:7124",
                                              "http://localhost:5124")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                      });
});

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

app.UseAuthorization();

app.MapControllers();

app.UseCors(AllowedOrigins);

app.Run();
