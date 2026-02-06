using NewLife.Caching;
using NewLife.Caching.Services;
using NewLife.Log;
using NewLife.Model;
using NewLife.Remoting;
using Stardust;
using XCode;
using Zero.RpcServer;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

// 初始化对象容器，提供依赖注入能力
var services = ObjectContainer.Current;

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
var star = services.AddStardust();

// 默认内存缓存，如有配置RedisCache可使用Redis缓存
services.AddSingleton<ICacheProvider, RedisCacheProvider>();

// 引入Redis，用于消息队列和缓存，单例，带性能跟踪。一般使用上面的ICacheProvider替代
//services.AddRedis("127.0.0.1:6379", "123456", 3, 5000);

EntityFactory.InitAll();

var port = 8080;

// 实例化RPC服务端，指定端口，同时在Tcp/Udp/IPv4/IPv6上监听
using var server = new ApiServer(port)
{
    Name = "银河服务端",

    // 指定编码器
    Encoder = new JsonEncoder(),

    //EncoderLog = XTrace.Log,
    Log = XTrace.Log,
    Tracer = star.Tracer,
};

// 注册服务控制器，其中提供各种接口服务
server.Register<MyController>();
server.Register<UserController>();
server.Register<AreaController>();

#if DEBUG
// 打开编码日志
server.EncoderLog = XTrace.Log;
#endif

// 启动网络服务，监听端口，所有逻辑将在 xxxController 中处理
server.Start();
XTrace.WriteLine("服务端启动完成！");

// 注册到星尘，非必须
star?.Service?.Register(star.AppId, () => $"tcp://*:{server.Port},udp://*:{server.Port}");

// 客户端测试，非服务端代码，正式使用时请注释掉
_ = Task.Run(() => ClientTest.TcpTest(port));
_ = Task.Run(() => ClientTest.UdpTest(port));
_ = Task.Run(() => ClientTest.WebSocketTest(port));
_ = Task.Run(() => ClientTest.HttpTest(port));

// 阻塞，等待友好退出
var host = services.BuildHost();
(host as Host).MaxTime = 10_000;
await host.RunAsync();
