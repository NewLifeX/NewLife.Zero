using NewLife;
using NewLife.Caching;
using NewLife.Cube;
using NewLife.Log;
using XCode;
using Zero.Web.Services;

//!!! 标准Web项目模板，新生命团队强烈推荐

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust(null);

// 启用星尘配置中心。分布式部署或容器化部署推荐使用，单机部署不推荐使用
var config = star.Config;

// 默认内存缓存，如有配置可使用Redis缓存
var cache = new MemoryCache();
if (config != null && !config["redisCache"].IsNullOrEmpty())
    services.AddSingleton<ICache>(p => new FullRedis(p, "redisCache") { Name = "Cache", Tracer = star.Tracer });
else
    services.AddSingleton<ICache>(cache);

// 启用接口响应压缩
services.AddResponseCompression();

services.AddControllersWithViews();

// 引入魔方
services.AddCube();

// 后台服务
services.AddHostedService<MyHostedService>();

var app = builder.Build();

// 预热数据层，执行自动建表等操作
// 连接名 Zero 对应连接字符串名字，同时也对应 Zero.Data/Projects/Model.xml 头部的 ConnName
EntityFactory.InitConnection("Zero");

// 使用Cube前添加自己的管道
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/CubeHome/Error");

app.UseResponseCompression();

// 使用魔方
app.UseCube(app.Environment);

app.UseAuthorization();

// 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
app.RegisterService("Zero.Web", null, app.Environment.EnvironmentName);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=CubeHome}/{action=Index}/{id?}");
});

app.Run();