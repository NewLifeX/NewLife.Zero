using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using Stardust;

namespace Zero.TcpServer
{
    internal class Program
    {
        private static async Task Main(String[] args)
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

            // 客户端测试，非服务端代码
            _ = Task.Run(ClientTest);

            // 阻塞，等待友好退出
            await ObjectContainer.Current.BuildHost().RunAsync();
        }

        private static async void ClientTest()
        {
            await Task.Delay(1_000);

            var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 12345);
            var ns = client.GetStream();

            // 接收服务端握手
            var buf = new Byte[1024];
            var count = await ns.ReadAsync(buf);
            XTrace.WriteLine("<={0}", buf.ToStr(null, 0, count));

            // 发送数据
            var str = "Hello NewLife";
            XTrace.WriteLine("=>{0}", str);
            await ns.WriteAsync(str.GetBytes());

            // 接收数据
            count = await ns.ReadAsync(buf);
            XTrace.WriteLine("<={0}", buf.ToStr(null, 0, count));
        }
    }
}