using APICodeMetrics.Configuration;
using APICodeMetrics.Interfaces;
using APICodeMetrics.Models.DTO;
using APICodeMetrics.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<SferaCodeApiConfig>(
    builder.Configuration.GetSection(SferaCodeApiConfig.SectionName));

builder.Services.AddHttpClient<ISferaCodeApiClient, SferaCodeApiClient>(client =>
{
    // BaseAddress будет установлен в конструкторе клиента
});

builder.Services.AddScoped<IDataCollector<ProjectDto[]>, ProjectCollector>();
builder.Services.AddScoped<IDataCollector<RepositoryDto[]>, RepositoryCollector>();
builder.Services.AddScoped<IDataCollector<BranchDto[]>, BranchCollector>();
builder.Services.AddScoped<IDataCollector<CommitDto[]>, CommitCollector>();
builder.Services.AddScoped<IDataCollector<CommitDetailsDto>, CommitDetailsCollector>();
builder.Services.AddScoped<IDataCollector<CommitDiffDto>, CommitDiffCollector>();

builder.Services.AddDbContext<APICodeMetrics.Data.ApiCodeMetricsContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DataCollectionOrchestrator>();

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