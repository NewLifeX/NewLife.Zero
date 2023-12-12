using NewLife.Caching.Services;
using NewLife.Caching;
using NewLife.Http;
using NewLife.Log;
using NewLife.Model;
using NewLife.Remoting;
using Stardust;
using Zero.HttpServer;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var services = ObjectContainer.Current;

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust();

// 默认内存缓存，如有配置RedisCache可使用Redis缓存
services.AddSingleton<ICacheProvider, RedisCacheProvider>();

// 引入Redis，用于消息队列和缓存，单例，带性能跟踪。一般使用上面的ICacheProvider替代
//services.AddRedis("127.0.0.1:6379", "123456", 3, 5000);

var server = new HttpServer
{
    Port = 8080,
    Log = XTrace.Log,
    //SessionLog = XTrace.Log,
    Tracer = star.Tracer,
};
server.Map("/", () => "<h1>Hello NewLife!</h1></br> " + DateTime.Now.ToFullString() + "</br><img src=\"logos/leaf.png\" />");
server.Map("/user", (String act, Int32 uid) => new { code = 0, data = $"User.{act}({uid}) success!" });
server.MapStaticFiles("/logos", "images/");
server.MapStaticFiles("/", "./");
server.MapController<ApiController>("/api");
server.Map("/my", new MyHttpHandler());
server.Start();

// 发布到星尘注册中心
await star.Service?.RegisterAsync("MyHttpServer", $"http://*:{server.Port}");

// 异步阻塞，友好退出
var host = services.BuildHost();
await host.RunAsync();
