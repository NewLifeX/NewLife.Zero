using System;
using System.Threading.Tasks;
using NewLife;
using NewLife.Log;
using Stardust;

namespace Zero.TcpServer
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            // 启用控制台日志，拦截所有异常
            XTrace.UseConsole();

            // 配置星尘。自动读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
            var star = new StarFactory(null, null, null);
            if (star.Server.IsNullOrEmpty()) star = null;

            // 实例化网络服务端，指定端口，同时在Tcp/Udp/IPv4/IPv6上监听
            var server = new MyNetServer
            {
                Port = 12345,
                Log = XTrace.Log,
                SessionLog = XTrace.Log,
                Tracer = star?.Tracer,

#if DEBUG
                SocketLog = XTrace.Log,
                LogSend = true,
                LogReceive = true,
#endif
            };

            // 启动网络服务，监听端口，所有逻辑将在 MyNetSession 中处理
            server.Start();

            // 注册到星尘，非必须
            star?.Service.Register("MyNetServer", () => $"tcp://*:{server.Port},udp://*:{server.Port}");

            // 退出事件
            var life = new TaskCompletionSource<Object>();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => life.TrySetResult(null);
            Console.CancelKeyPress += (s, e) => life.TrySetResult(null);
            life.Task.Wait();
        }
    }
}