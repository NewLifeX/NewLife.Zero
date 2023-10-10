using NewLife;
using NewLife.Log;
using NewLife.Model;
using NewLife.Remoting;
using Stardust;
using Zero.RpcServer;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var services = ObjectContainer.Current;

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
var star = services.AddStardust();

// 实例化RPC服务端，指定端口，同时在Tcp/Udp/IPv4/IPv6上监听
var server = new ApiServer(12346)
{
    // 指定编码器
    Encoder = new JsonEncoder(),

    //EncoderLog = XTrace.Log,
    Log = XTrace.Log,
    Tracer = star.Tracer,
};

// 注册服务控制器
server.Register<MyController>();
server.Register<ProductController>();

#if DEBUG
// 打开编码日志
server.EncoderLog = XTrace.Log;
#endif

server.Start();

// 注册到星尘，非必须
star?.Service?.Register("MyRpcServer", () => $"tcp://*:{server.Port},udp://*:{server.Port}");

// 阻塞，等待友好退出
var host = services.BuildHost();
await host.RunAsync();
