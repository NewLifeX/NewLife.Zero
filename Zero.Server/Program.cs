using NewLife.Caching;
using NewLife.Caching.Services;
using NewLife.Log;
using NewLife.Model;
using Stardust;
using Zero.Server;

// 初始化对象容器，提供依赖注入能力
var services = ObjectContainer.Current;
services.AddSingleton(XTrace.Log);

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust();

// 默认内存缓存，如有配置RedisCache可使用Redis缓存
services.AddSingleton<ICacheProvider, RedisCacheProvider>();

var svc = new MyServices { ServiceProvider = services.BuildServiceProvider() };
svc.Main(args);
