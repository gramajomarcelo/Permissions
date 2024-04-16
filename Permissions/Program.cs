using Microsoft.EntityFrameworkCore;
using Permissions.BL.Data;
using Permissions.BL.Models;
using Permissions.BL.Repositories;
using Permissions.BL.Repositories.Implements;
using Permissions.BL.Repositories.UnitOfWork.Implements;
using Permissions.BL.Repositories.UnitOfWork;
using Permissions.BL.Services;
using Permissions.BL.Services.Implements;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.Elasticsearch;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRequestPermissionService, RequestPermissionService>();
builder.Services.AddScoped<IModifyPermissionService, ModifyPermissionService>();
builder.Services.AddScoped<IGetPermissionsService, GetPermissionsService>();
builder.Services.AddScoped<IGenericReadRepository<Permission>, GenericReadRepository<Permission>>();
builder.Services.AddScoped<IGenericWriteRepository<Permission>, GenericWriteRepository<Permission>>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configurar la conexión a Kafka
var config = new ProducerConfig
{
    BootstrapServers = builder.Configuration.GetValue<string>("KafkaConfig:BootstrapServers")
};

// Registrar el productor de Kafka como un servicio para inyectarlo en otros componentes
builder.Services.AddSingleton(new ProducerBuilder<string, string>(config).Build());

builder.Services.AddLogging(logginBuilder =>
{
    var elasticSerachNodeUri = builder.Configuration.GetValue<string>("LoggingOptions:NodeUri");

    var loggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] - {Message} {Properties} {Newline}"
        )
        .WriteTo.File(
            "PermissionsAPI-logs.txt",
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] - {Message} {Properties} {Newline}"
        )
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSerachNodeUri))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"Permissions-Logs-{DateTime.Now:yyyy.MM.dd}"
        });

    var logger = loggerConfiguration.CreateLogger();
    logginBuilder.Services.AddSingleton<ILoggerFactory>(
        provider => new SerilogLoggerFactory(logger, dispose: false));
});
// Configurar la cadena de conexión de la base de datos desde appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configurar el contexto de la base de datos
builder.Services.AddDbContext<PermissionsContext>(options =>
{
    options.UseSqlServer(connectionString);
});
// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

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

app.Run();
