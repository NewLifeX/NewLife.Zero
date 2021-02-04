using System;
using AntJob;
using AntJob.Providers;
using NewLife;
using NewLife.Agent;
using NewLife.Log;
using NewLife.Remoting;
using Stardust.Monitors;
using XCode.DataAccessLayer;

namespace Zero.AntJob
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
            ServiceName = "AntAgent";

            DisplayName = "大数据计算";
            Description = "蚂蚁调度系统子程序，编写数据处理等业务逻辑，连接蚂蚁调度中心，拉取作业任务来执行";

            // 注册菜单，在控制台菜单中按 t 可以执行Test函数，主要用于临时处理数据
            AddMenu('t', "数据测试", Test);
        }
        #endregion

        #region 核心
        private Scheduler _Scheduler;
        /// <summary>开始工作</summary>
        /// <param name="reason"></param>
        protected override void StartWork(String reason)
        {
            WriteLog("业务开始……");

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

            var set2 = AntSetting.Current;

            // 实例化调度器
            var sc = new Scheduler
            {
                Tracer = Tracer,

                // 使用分布式调度引擎替换默认的本地文件调度
                Provider = new NetworkJobProvider
                {
                    Debug = set2.Debug,
                    Server = set2.Server,
                    AppID = set2.AppID,
                    Secret = set2.Secret,
                }
            };

            // 添加作业处理器
            //sc.Handlers.Add(new SqlHandler());
            //sc.Handlers.Add(new SqlMessage());
            sc.Handlers.Add(new HelloJob());
            sc.Handlers.Add(new BuildProduct());
            sc.Handlers.Add(new BuildPlan());

            // 启动调度引擎，调度器内部多线程处理
            sc.Start();

            _Scheduler = sc;

            base.StartWork(reason);
        }

        /// <summary>停止服务</summary>
        /// <param name="reason"></param>
        protected override void StopWork(String reason)
        {
            base.StopWork(reason);

            _Scheduler.TryDispose();
            _Scheduler = null;
        }

        /// <summary>数据测试，菜单t</summary>
        public void Test()
        {
        }
        #endregion
    }
}