using NewLife;
using NewLife.Caching;
using NewLife.Log;
using Zero.Worker;
using Zero.Worker.Services;

//!!! 标准Worker模板，可以使用完整的IOC

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // 初始化配置文件
        InitConfig();

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
            services.AddRedis("127.0.0.1:6379", "123456", 3, 5000);
        }

        // 注册后台服务
        services.AddHostedService<Worker>();
        services.AddHostedService<RedisWorker>();
        //services.AddHostedService<MqttWorker>();
        //services.AddHostedService<RocketMqWorker>();
        // 先预热数据，再启动Web服务，避免网络连接冲击
        services.AddHostedService<PreheatHostedService>();
    })
    .Build();

// 注册退出事件
NewLife.Model.Host.RegisterExit(() => host.StopAsync().Wait());

await host.RunAsync();

static void InitConfig()
{
    // 配置
    var set = NewLife.Setting.Current;
    if (set.IsNew)
    {
        set.LogPath = "../LogWorker";
        set.DataPath = "../Data";
        set.BackupPath = "../Backup";
        set.Save();
    }
    //var set2 = XCode.Setting.Current;
    //if (set2.IsNew)
    //{
    //    // 关闭SQL日志输出
    //    set2.ShowSQL = false;
    //    set2.Save();
    //}
}
