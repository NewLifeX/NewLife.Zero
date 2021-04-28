using System;
using System.Threading.Tasks;
using AntJob;
using AntJob.Providers;
using NewLife;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Model;
using Stardust;
using XCode.DataAccessLayer;

namespace Zero.AntJob
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            XTrace.UseConsole();

            var services = ObjectContainer.Current;

            var star = new StarFactory(null, null, null);

            // 数据库连接配置
            DAL.SetConfig(star.Config);

            // 配置缓存，使用 MemoryCache 或 Redis，其中Redis从配置中心读取配置信息
            var cache = new MemoryCache { Capacity = 1_000_000 };
            services.AddSingleton<ICache>(cache);
            //services.AddSingleton<ICache>(p => new FullRedis(p, "redisCache"));

            // 配置蚂蚁调度
            var set = AntSetting.Current;
            var server = star.Config["antServer"];
            if (!server.IsNullOrEmpty())
            {
                set.Server = server;
                set.Save();
            }

            // 实例化调度器
            var sc = new Scheduler
            {
                Tracer = star.Tracer,

                // 使用分布式调度引擎替换默认的本地文件调度
                Provider = new NetworkJobProvider
                {
                    Server = set.Server,
                    AppID = set.AppID,
                    Secret = set.Secret,
                    Debug = false
                }
            };

            // 添加作业，作业支持IObjectContainer的构造参数注入
            sc.AddHandler<HelloJob>();
            sc.AddHandler<BuildProduct>();
            sc.AddHandler<BuildPlan>();

            // 启动调度引擎，调度器内部多线程处理
            sc.Start();
            
            // 后台任务
            var life = new TaskCompletionSource<Object>();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => life.TrySetResult(null);
            Console.CancelKeyPress += (s, e) => life.TrySetResult(null);
            life.Task.Wait();

            sc.TryDispose();
        }
    }
}