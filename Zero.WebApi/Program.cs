using NewLife;
using NewLife.Caching;
using NewLife.Log;
using XCode;

//!!! 标准WebApi项目模板，新生命团队强烈推荐

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust(null);

{
    // 启用星尘配置中心。分布式部署或容器化部署推荐使用，单机部署不推荐使用
    var config = star.Config;

    // 默认内存缓存，如有配置可使用Redis缓存
    var cache = new MemoryCache();
    if (config != null && !config["redisCache"].IsNullOrEmpty())
        services.AddSingleton<ICache>(p => new FullRedis(p, "redisCache") { Name = "Cache" });
    else
        services.AddSingleton<ICache>(cache);
}

{
    // 引入Redis，用于消息队列和缓存，单例，带性能跟踪
    var rds = new FullRedis { Tracer = star.Tracer };
    rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
    //services.AddSingleton<ICache>(rds);
    services.AddSingleton(rds);
}

builder.Services.AddControllers();

// 引入 Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 预热数据层，执行反向工程建表等操作
EntityFactory.InitConnection("Membership");
EntityFactory.InitConnection("Zero");

// 使用星尘，启用性能监控，拦截所有接口做埋点统计
app.UseStardust();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
app.RegisterService("Zero.WebApi", null, "dev");

app.Run();
