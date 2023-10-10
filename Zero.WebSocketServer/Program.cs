using NewLife.Http;
using NewLife.Log;
using NewLife.Model;
using Stardust;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var services = ObjectContainer.Current;

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust();

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
