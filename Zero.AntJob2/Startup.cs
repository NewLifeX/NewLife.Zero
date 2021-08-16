using System.Collections.Generic;
using AntJob;
using AntJob.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLife;
using NewLife.Common;
using NewLife.Log;
using Zero.AntJob2.Jobs;
using Zero.AntJob2.Services;

namespace Zero.AntJob2
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            // 注册服务
            services.AddSingleton<ITestService, TestService>();


            // 注册任务处理器
            services.AddSingleton<Handler, TestJob>();

            // 注册任务提供者
            services.AddSingleton<IJobProvider>(fact =>
            {
                var cfg = fact.GetRequiredService<IConfiguration>();
                var server = cfg["AntServer"];

                // 配置蚂蚁调度
                var set = AntSetting.Current;
                if (!server.IsNullOrEmpty())
                {
                    set.Server = server;
                    set.Save();
                }


                return new NetworkJobProvider
                {
                    Server = set.Server,
                    AppID = set.AppID,
                    Secret = set.Secret,
                    Debug = false
                };
            });

            // 注册调度器
            services.AddSingleton(fact =>
            {
                var provider = fact.GetRequiredService<IJobProvider>();
                var handlers = fact.GetRequiredService<IEnumerable<Handler>>();
                //var tracer = fact.GetRequiredService<ITracer>();

                // 使用分布式调度引擎替换默认的本地文件调度
                var scheduler = new Scheduler
                {
                    Provider = provider,
                    //Tracer = tracer
                };

                // 添加调度作业
                scheduler.Handlers.AddRange(handlers);

                return scheduler;
            });
        }
    }
}
