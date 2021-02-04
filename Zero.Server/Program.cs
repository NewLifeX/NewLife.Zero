using System;
using NewLife;
using NewLife.Agent;
using NewLife.Log;
using NewLife.Net;
using NewLife.Remoting;
using NewLife.Threading;
using Stardust.Monitors;
using XCode.DataAccessLayer;

namespace Zero.Server
{
    internal class Program
    {
        private static void Main(String[] args) => new MyServices().Main(args);
    }

    /// <summary>代理服务例子。自定义服务程序可参照该类实现。</summary>
    public class MyServices : ServiceBase
    {
        #region 属性
        /// <summary>性能跟踪器</summary>
        public ITracer Tracer { get; set; }
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
        MyNetServer _netServer;
        ApiServer _rpcServer;

        /// <summary>开始工作</summary>
        /// <param name="reason"></param>
        protected override void StartWork(String reason)
        {
            // 配置APM性能跟踪器
            var set = Stardust.Setting.Current;
            if (!set.Server.IsNullOrEmpty())
            {
                // 配置指向星尘监控中心
                var tracer = new StarTracer(set.Server) { Log = XTrace.Log };
                DefaultTracer.Instance = tracer;
                ApiHelper.Tracer = tracer;
                DAL.GlobalTracer = tracer;
                Tracer = tracer;
            }

            InitNetServer();
            InitRpcServer();

            _timer1 = new TimerX(s => ShowStat(), null, 1000, 1000) { Async = true };
            _timer2 = new TimerX(s => SendTime(_netServer), null, 1000, 1000) { Async = true };

            base.StartWork(reason);
        }

        void InitNetServer()
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

            _netServer = svr;
        }

        void InitRpcServer()
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

            _rpcServer = svr;
        }

        /// <summary>停止服务</summary>
        /// <param name="reason"></param>
        protected override void StopWork(String reason)
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
}