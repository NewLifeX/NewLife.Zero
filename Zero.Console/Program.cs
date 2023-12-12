using NewLife.Caching;
using NewLife.Caching.Services;
using Stardust;
using Zero.Console;
using Zero.Console.Workers;

//!!! 标准后台服务项目模板，新生命团队强烈推荐

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

// 初始化对象容器，提供注入能力
var services = ObjectContainer.Current;
services.AddSingleton(XTrace.Log);

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust();

// 默认内存缓存，如有配置RedisCache可使用Redis缓存
services.AddSingleton<ICacheProvider, RedisCacheProvider>();

// 初始化Redis、MQTT、RocketMQ，注册服务到容器
services.AddRedis(star?.Tracer);
services.AddMqtt(star?.Tracer);
services.AddRocketMQ(star?.Tracer);

// 注册后台任务 IHostedService
services.AddHostedService<Worker>();

var host = services.BuildHost();

// 异步阻塞，友好退出
await host.RunAsync();
