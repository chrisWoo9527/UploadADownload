using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Service.AutoFacManager;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration().
    MinimumLevel.Information().
    WriteTo.File($"{AppContext.BaseDirectory}00_Logs\\log.log", rollingInterval: RollingInterval.Day)
             .CreateLogger();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//ע��serilog����
builder.Services.AddLogging(loggingBuilder =>
          loggingBuilder.AddSerilog(dispose: true));


// �滻���õ�ServiceProviderFactory
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// �Զ�ע�����
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    AutofacExtend.UseCustomConfigureContainer(containerBuilder);
});

// �����йܷ�������С10G
builder.Services.Configure<FormOptions>(x =>
{
    x.MultipartBodyLengthLimit = (long)1024 * 1024 * 1024;
    x.ValueCountLimit = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(x =>
{
    x.Limits.MaxRequestBodySize = (long)1024 * 1024 * 1024;
});

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
