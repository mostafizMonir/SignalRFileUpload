using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Order.Notifications;
using Order.Notifications.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

// Add Redis configuration
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

// Add IDistributedCache using Redis
builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

// Then in middleware:


var app = builder.Build();


// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};



app.MapPost("orders", async (IDistributedCache cache, IHubContext<OrderNotificationHub> hubContext) =>
{
    var order = new DummyOrder()
    {
        Id = Guid.NewGuid(),
        Name = "Monir order 1"

    };
    var cacheKey = $"order:{order.Id}";
    var options = new DistributedCacheEntryOptions()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
    };

    await cache.SetStringAsync(
        cacheKey, JsonSerializer.Serialize(order),
        options
        );

    await hubContext.Clients.All.SendAsync("OrderCreated", order);

    return Results.Created($"orders/{order.Id}", order);
});

app.MapGet("orders/{id}", async (Guid id, IDistributedCache cache) =>
{
    var cacheKey = $"order:{id}";
    var orderJson = await cache.GetStringAsync(cacheKey);
    
    if (orderJson == null)
    {
        return Results.NotFound();
    }

    var order = JsonSerializer.Deserialize<DummyOrder>(orderJson);
    return Results.Ok(order);
});
app.MapGet("orders", async (IConnectionMultiplexer redis, IDistributedCache cache) =>
{
    var orders = new List<DummyOrder>();
    var endpoints = redis.GetEndPoints();
    var server = redis.GetServer(endpoints.First());
    var keys = server.Keys(pattern: "order:*");

    foreach (var key in keys)
    {
        var orderJson = await cache.GetStringAsync(key);
        if (orderJson != null)
        {
            var order = JsonSerializer.Deserialize<DummyOrder>(orderJson);
            orders.Add(order);
        }
    }

    return Results.Ok(orders);
});

app.MapPut("orders/{id}", async (Guid id, DummyOrder order, 
    IDistributedCache cache, IHubContext<OrderNotificationHub> hubContext) =>
{
    if (id != order.Id)
    {
        return Results.BadRequest();
    }

    var cacheKey = $"order:{id}";
    var options = new DistributedCacheEntryOptions()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
    };

    await cache.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(order),
        options
    );

    await hubContext.Clients.All.SendAsync("OrderStatusUpdated", order);

    return Results.NoContent();
});


app.MapHub<OrderNotificationHub>("/orderHub");

app.Run();
