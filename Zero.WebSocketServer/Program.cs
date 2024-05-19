using System.Net.WebSockets;
using NewLife;
using NewLife.Caching;
using NewLife.Caching.Services;
using NewLife.Data;
using NewLife.Http;
using NewLife.Log;
using NewLife.Model;
using Stardust;

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
    Port = 8081,
    Log = XTrace.Log,
    //SessionLog = XTrace.Log,
    Tracer = star.Tracer,
};
server.Map("/ws", new WebSocketHandler());
server.Start();

// 发布到星尘注册中心
await star.Service?.RegisterAsync("MyWebSocketServer", $"ws://*:{server.Port}");

#if DEBUG
var client = new ClientWebSocket();
await client.ConnectAsync(new Uri("ws://127.0.0.1:8081/ws"), default);
await client.SendAsync("Hello NewLife".GetBytes(), System.Net.WebSockets.WebSocketMessageType.Text, true, default);

var buf = new Byte[1024];
var rs = await client.ReceiveAsync(buf, default);
XTrace.WriteLine(new Packet(buf, 0, rs.Count).ToStr());

await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "通信完成", default);
XTrace.WriteLine("Close [{0}] {1}", client.CloseStatus, client.CloseStatusDescription);
#endif

// 异步阻塞，友好退出
var host = services.BuildHost();
await host.RunAsync();
