using System.Text.Json;
using JackBallers.Api.Models;
using JackBallers.Api.Strike;
using StackExchange.Redis;

namespace JackBallers.Api;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddRouting();
        builder.Services.AddHostedService<BallerFactory>();
        
        var redis = builder.Configuration.GetSection("Redis").Get<RedisConfig>();
        var cx = await ConnectionMultiplexer.ConnectAsync(redis.Connection);
        builder.Services.AddSingleton(cx.GetDatabase());

        var partnerConfig = builder.Configuration.GetSection("StrikeApi").Get<PartnerApiSettings>();
        builder.Services.AddSingleton(partnerConfig);
        builder.Services.AddTransient<PartnerApi>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseEndpoints(ep =>
        {
            ep.MapControllers();
            ep.MapFallbackToFile("index.html");
        });
        await app.RunAsync();
    }
}

public class RedisConfig
{
    public string Connection { get; init; }
}