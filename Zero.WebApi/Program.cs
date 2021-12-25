using NewLife;
using NewLife.Caching;
using NewLife.Log;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
var star = services.AddStardust(null);
var config = star.Config;

// 默认内存缓存，如有配置可使用Redis缓存
var cache = new MemoryCache();
if (config != null && !config["redisCache"].IsNullOrEmpty())
    services.AddSingleton<ICache>(p => new FullRedis(p, "redisCache") { Name = "Cache" });
else
    services.AddSingleton<ICache>(cache);

// 引入Redis，用于消息队列和缓存，单例，带性能跟踪
var rds = new FullRedis { Tracer = star.Tracer };
rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
//services.AddSingleton<ICache>(rds);
services.AddSingleton(rds);

builder.Services.AddControllers();

// 引入 Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 使用星尘，拦截所有接口做埋点统计
app.UseStardust();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
