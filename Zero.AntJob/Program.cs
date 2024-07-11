using AntJob;
using NewLife.Caching;
using NewLife.Caching.Services;
using NewLife.Log;
using NewLife.Model;
using Stardust;
using Zero.AntJob;
using Zero.AntJob.Services;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var services = ObjectContainer.Current;
services.AddStardust();

services.AddSingleton(AntSetting.Current);

// 默认内存缓存，如有配置RedisCache可使用Redis缓存
services.AddSingleton<ICacheProvider, RedisCacheProvider>();

// 注册服务
services.AddSingleton<ITestService, TestService>();

// 注册后台服务
services.AddHostedService<AntJobWorker>();

// 友好退出
var host = services.BuildHost();
await host.RunAsync();