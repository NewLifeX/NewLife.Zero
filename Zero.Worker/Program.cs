using NewLife;
using NewLife.Caching;
using NewLife.Log;
using Zero.Worker;

//!!! 标准Worker模板，可以使用完整的IOC

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址
        var star = services.AddStardust(null);

        {
            // 启用星尘配置中心。分布式部署或容器化部署推荐使用，单机部署不推荐使用
            var config = star.Config;

            // 默认内存缓存，如有配置可使用Redis缓存
            var cache = new MemoryCache();
            if (config != null && !config["redisCache"].IsNullOrEmpty())
                services.AddSingleton<ICache>(p => new FullRedis(p, "redisCache") { Name = "Cache" });
            else
                services.AddSingleton<ICache>(cache);
        }

        {
            // 引入Redis，用于消息队列和缓存，单例，带性能跟踪
            var rds = new FullRedis { Tracer = star.Tracer };
            rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
            //services.AddSingleton<ICache>(rds);
            services.AddSingleton(rds);
        }

        // 注册后台服务
        services.AddHostedService<Worker>();
        services.AddHostedService<RedisWorker>();
        //services.AddHostedService<MqttWorker>();
        //services.AddHostedService<RocketMqWorker>();
    })
    .Build();

await host.RunAsync();
