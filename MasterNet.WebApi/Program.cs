using MasterNet.Application;
using MasterNet.Application.Interfaces;
using MasterNet.Infrastructure.Reports;
using MasterNet.Persistence;
using MasterNet.WebApi.Extensions;
using MasterNet.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddPersistence(builder.Configuration);

// builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped(typeof(IReportService<>), typeof(ReportService<>));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

await app.SeedDataAuthenticacion();

app.MapControllers();

app.Run();
