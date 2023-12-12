using AntJob;
using AntJob.Providers;
using NewLife;
using NewLife.Caching;
using NewLife.Caching.Services;
using NewLife.Log;
using Zero.AntJob;
using Zero.AntJob.Jobs;
using Zero.AntJob.Services;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址
        var star = services.AddStardust(null);
       
        // 默认内存缓存，如有配置RedisCache可使用Redis缓存
        services.AddSingleton<ICacheProvider, RedisCacheProvider>();

        // 引入Redis，用于消息队列和缓存，单例，带性能跟踪。一般使用上面的ICacheProvider替代
        //services.AddRedis("127.0.0.1:6379", "123456", 3, 5000);

        // 注册服务
        services.AddSingleton<ITestService, TestService>();

        // 注册任务处理器
        services.AddSingleton<Handler, TestJob>();
        services.AddSingleton<Handler, HelloJob>();
        services.AddSingleton<Handler, BuildProduct>();
        services.AddSingleton<Handler, BuildPlan>();

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
            var tracer = fact.GetService<ITracer>();

            // 使用分布式调度引擎替换默认的本地文件调度
            var scheduler = new Scheduler
            {
                Provider = provider,
                Tracer = tracer
            };

            // 添加调度作业
            scheduler.Handlers.AddRange(handlers);

            return scheduler;
        });

        // 注册后台服务
        services.AddHostedService<AntJobWorker>();
    })
    .Build();

await host.RunAsync();