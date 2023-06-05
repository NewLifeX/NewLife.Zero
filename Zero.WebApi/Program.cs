using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Serialization;
using XCode;
using Zero.WebApi;
using Zero.WebApi.Services;

//!!! 标准WebApi项目模板，新生命团队强烈推荐

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// 初始化配置文件
InitConfig();

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
    services.AddRedis("127.0.0.1:6379", "123456", 3, 5000);
}

var set = ApiSetting.Current;
services.AddSingleton(set);

services.AddSingleton<NodeService>();

// 启用接口响应压缩
services.AddResponseCompression();

// 配置Json
services.Configure<JsonOptions>(options =>
{
#if NET7_0_OR_GREATER
    // 支持模型类中的DataMember特性
    options.JsonSerializerOptions.TypeInfoResolver = DataMemberResolver.Default;
#endif
    options.JsonSerializerOptions.Converters.Add(new TypeConverter());
    options.JsonSerializerOptions.Converters.Add(new LocalTimeConverter());
    // 支持中文编码
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

services.AddControllers();

// 引入 Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 后台服务
services.AddHostedService<MyHostedService>();

var app = builder.Build();

// 预热数据层，执行自动建表等操作
// 连接名 Zero 对应连接字符串名字，同时也对应 Zero.Data/Projects/Model.xml 头部的 ConnName
EntityFactory.InitConnection("Zero");

app.UseResponseCompression();

// 使用星尘，启用性能监控，拦截所有接口做埋点统计
app.UseStardust();

// Configure the HTTP request pipeline.
// 注意：生产环境swagger会被禁用，如需要在生产环境启用sw需要取消环境判断参数
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// 注册退出事件
if (app is IHost host)
    NewLife.Model.Host.RegisterExit(() => host.StopAsync().Wait());

// 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
// 如不使用星尘的注册中心，可以注释该行代码
app.RegisterService("Zero.WebApi", null, app.Environment.EnvironmentName);

app.Run();

static void InitConfig()
{
    // 配置
    var set = NewLife.Setting.Current;
    if (set.IsNew)
    {
        set.LogPath = "../LogApi";
        set.DataPath = "../Data";
        set.BackupPath = "../Backup";
        set.Save();
    }
    //var set2 = XCode.Setting.Current;
    //if (set2.IsNew)
    //{
    //    // 关闭SQL日志输出
    //    set2.ShowSQL = false;
    //    set2.Save();
    //}
}
