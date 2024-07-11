using System;
using NewLife;
using NewLife.Agent;
using NewLife.Caching.Services;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Model;
using NewLife.Net;
using NewLife.Remoting;
using NewLife.Threading;
using Stardust;

namespace Zero.Server;

internal class Program
{
    private static void Main(String[] args)
    {
        // 初始化对象容器，提供注入能力
        var services = ObjectContainer.Current;
        services.AddSingleton(XTrace.Log);

        // 配置星尘。自动读取配置文件 config/star.config 中的服务器地址
        var star = services.AddStardust();

        // 默认内存缓存，如有配置RedisCache可使用Redis缓存
        services.AddSingleton<ICacheProvider, RedisCacheProvider>();

        var svc = new MyServices { ServiceProvider = services.BuildServiceProvider() };
        svc.Main(args);
    }
}

/// <summary>代理服务例子。自定义服务程序可参照该类实现。</summary>
public class MyServices : ServiceBase
{
    #region 属性
    public IServiceProvider ServiceProvider { get; set; }

    /// <summary>性能跟踪器</summary>
    public ITracer Tracer { get; set; }

    private StarFactory _star;
    #endregion

    #region 构造函数
    /// <summary>实例化一个代理服务</summary>
    public MyServices()
    {
        // 一般在构造函数里面指定服务名
        ServiceName = "EchoAgent";
        DisplayName = "回声服务";
        Description = "这是NewLife.Net的一个回声服务示例！";
    }
    #endregion

    #region 核心
    private MyNetServer _netServer;
    private ApiServer _rpcServer;

    /// <summary>开始工作</summary>
    /// <param name="reason"></param>
    public override void StartWork(String reason)
    {
        // 配置星尘。自动读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
        _star = ServiceProvider.GetService<StarFactory>();
        Tracer = _star.Tracer;

        InitNetServer();
        InitRpcServer();

        _timer1 = new TimerX(s => ShowStat(), null, 1000, 1000) { Async = true };
        _timer2 = new TimerX(s => SendTime(_netServer), null, 1000, 1000) { Async = true };

        base.StartWork(reason);
    }

    private void InitNetServer()
    {
        // 实例化网络服务端，指定端口，同时在Tcp/Udp/IPv4/IPv6上监听
        var svr = new MyNetServer
        {
            Port = 12345,
            Log = XTrace.Log,
            Tracer = Tracer,
#if DEBUG
            SocketLog = XTrace.Log,
            LogSend = true,
            LogReceive = true,
#endif
        };
        svr.Start();

        // 注册到星尘，非必须
        _star.Service?.Register("MyNetServer", () => $"tcp://*:{svr.Port},udp://*:{svr.Port}");

        _netServer = svr;
    }

    private void InitRpcServer()
    {
        // 实例化RPC服务端，指定端口，同时在Tcp/Udp/IPv4/IPv6上监听
        var svr = new ApiServer(12346)
        {
            // 指定编码器
            Encoder = new JsonEncoder(),

            EncoderLog = XTrace.Log,
            Log = XTrace.Log,
            Tracer = Tracer,
        };

        // 注册服务控制器
        svr.Register<MyController>();
        svr.Register<ProductController>();

#if DEBUG
        // 打开原始数据日志
        var ns = svr.EnsureCreate() as NetServer;
        //var ns = svr.Server as NetServer;
        ns.Log = XTrace.Log;
        ns.LogSend = true;
        ns.LogReceive = true;
#endif

        svr.Start();

        // 注册到星尘，非必须
        _star.Service?.Register("MyRpcServer", () => $"tcp://*:{svr.Port},udp://*:{svr.Port}");

        _rpcServer = svr;
    }

    /// <summary>停止服务</summary>
    /// <param name="reason"></param>
    public override void StopWork(String reason)
    {
        _netServer.TryDispose();
        _netServer = null;

        _rpcServer.TryDispose();
        _rpcServer = null;

        base.StopWork(reason);
    }

    private TimerX _timer1;
    private TimerX _timer2;

    private String _last;
    /// <summary>显示服务端状态</summary>
    private void ShowStat()
    {
        var ns = _netServer;
        var rs = _rpcServer?.Server as NetServer;
        if (ns == null && rs == null) return;

        var msg = ns?.GetStat() + " " + rs?.GetHashCode();
        if (msg == _last) return;

        _last = msg;

        WriteLog(msg);
    }

    /// <summary>向所有客户端发送时间</summary>
    /// <param name="ns"></param>
    private void SendTime(NetServer ns)
    {
        if (ns == null) return;

        var str = DateTime.Now.ToFullString() + Environment.NewLine;
        var buf = str.GetBytes();
        ns.SendAllAsync(buf);
    }
    #endregion
}