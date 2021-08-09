using System;
using System.Threading;
using System.Threading.Tasks;
using NewLife;
using NewLife.Log;
using NewLife.Remoting;
using Stardust;

namespace Zero.RpcServer
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

            // 实例化RPC服务端，指定端口，同时在Tcp/Udp/IPv4/IPv6上监听
            var server = new ApiServer(12346)
            {
                // 指定编码器
                Encoder = new JsonEncoder(),

                //EncoderLog = XTrace.Log,
                Log = XTrace.Log,
                Tracer = star?.Tracer,
            };

            // 注册服务控制器
            server.Register<MyController>();
            server.Register<ProductController>();

#if DEBUG
            // 打开编码日志
            server.EncoderLog = XTrace.Log;
#endif

            server.Start();

            star?.Service.Register("MyRpcServer", () => $"tcp://*:{server.Port},udp://*:{server.Port}");

            // 友好退出
            //ObjectContainer.Current.BuildHost().Run();
            Thread.Sleep(-1);
        }
    }
}