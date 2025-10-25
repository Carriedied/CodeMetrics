using APICodeMetrics.Configuration;
using APICodeMetrics.Interfaces;
using APICodeMetrics.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<SferaCodeApiConfig>(
    builder.Configuration.GetSection(SferaCodeApiConfig.SectionName));

builder.Services.AddHttpClient<ISferaCodeApiClient, SferaCodeApiClient>(client =>
{
    // BaseAddress будет установлен в конструкторе клиента
});

builder.Services.AddScoped<ISferaCodeApiClient, SferaCodeApiClient>();
builder.Services.AddScoped<IProjectCollectorService, ProjectCollectorService>();
builder.Services.AddScoped<IRepositoryCollectorService, RepositoryCollectorService>();
builder.Services.AddScoped<IGitMetricsCollector, GitMetricsService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();