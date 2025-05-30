﻿using NewLife.Cube;
using NewLife.Cube.Swagger;
using NewLife.Log;
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

// 默认内存缓存，如有配置RedisCache可使用Redis缓存
//services.AddSingleton<ICacheProvider, RedisCacheProvider>();
services.AddRedis();

// 引入Redis，用于消息队列和缓存，单例，带性能跟踪。一般使用上面的ICacheProvider替代
//services.AddRedis("127.0.0.1:6379", "123456", 3, 5000);

// 注入应用配置
var set = ApiSetting.Current;
services.AddSingleton(set);

// 注册Remoting所必须的服务
services.AddIoT(set);
//services.AddRemoting(set);

// 注入多个功能服务
services.AddSingleton<NodeService>();

// 启用接口响应压缩
services.AddResponseCompression();

services.AddControllers();

// 引入魔方框架，包含Swagger、OAuth等
services.AddCubeSwagger();
services.AddCube();

// 后台服务
services.AddHostedService<MyHostedService>();
// 先预热数据，再启动Web服务，避免网络连接冲击
services.AddHostedService<PreheatHostedService>();

var app = builder.Build();

// 预热数据层，执行自动建表等操作
_ = EntityFactory.InitAllAsync();

if (Environment.GetEnvironmentVariable("__ASPNETCORE_BROWSER_TOOLS") is null)
    app.UseResponseCompression();

app.UseIoT();

// 使用星尘，启用性能监控，拦截所有接口做埋点统计
app.UseStardust();

// Configure the HTTP request pipeline.
// 注意：生产环境swagger会被禁用，如需要在生产环境启用sw需要取消环境判断参数
if (app.Environment.IsDevelopment())
{
    app.UseCubeSwagger();
}

// 使用魔方框架
app.UseCube(builder.Environment);

app.UseAuthorization();

app.MapControllers();

// 注册退出事件
if (app is IHost host)
    NewLife.Model.Host.RegisterExit(() => host.StopAsync().Wait());

// 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
// 如不使用星尘的注册中心，可以注释该行代码
app.RegisterService(star.AppId, null, app.Environment.EnvironmentName);

app.Run();

static void InitConfig()
{
    // 把数据目录指向上层，例如部署到 /root/iot/edge/，这些目录放在 /root/iot/
    var set = NewLife.Setting.Current;
    if (set.IsNew)
    {
        set.LogPath = "../LogApi";
        set.DataPath = "../Data";
        set.BackupPath = "../Backup";
        set.Save();
    }
    var set2 = CubeSetting.Current;
    if (set2.IsNew)
    {
        set2.AvatarPath = "../Avatars";
        set2.UploadPath = "../Uploads";
        set2.Save();
    }
    var set3 = XCodeSetting.Current;
    if (set3.IsNew)
    {
        // 关闭SQL日志输出
        set3.ShowSQL = false;
        //set3.EntityCacheExpire = 60;
        //set3.SingleCacheExpire = 60;
        set3.Save();
    }
}
