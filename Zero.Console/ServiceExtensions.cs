using NewLife.Caching;
using NewLife.MQTT;
using NewLife.RocketMQ;
using Zero.Console.Workers;

namespace Zero.Console;

static class ServiceExtensions
{
    public static void AddRedis(this IObjectContainer services, ITracer tracer)
    {
        // 引入 Redis，用于消息队列和缓存，单例，带性能跟踪
        var rds = new FullRedis
        {
            Tracer = tracer,
            Log = XTrace.Log,
        };
        rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
        services.AddSingleton<ICache>(rds);
        services.AddSingleton(rds);

        services.AddHostedService<RedisWorker>();
    }

    public static void AddMqtt(this IObjectContainer services, ITracer tracer)
    {
        // 引入 MQTT
        var mqtt = new MqttClient
        {
            Tracer = tracer,
            Log = XTrace.Log,

            Server = "tcp://127.0.0.1:1883",
            ClientId = Environment.MachineName,
            UserName = "stone",
            Password = "Pass@word",
        };
        services.AddSingleton(mqtt);

        //services.AddHostedService<MqttWorker>();
    }

    public static void AddRocketMQ(this IObjectContainer services, ITracer tracer)
    {
        // 引入 RocketMQ 生产者
        var producer = new Producer
        {
            Topic = "nx_test",
            NameServerAddress = "127.0.0.1:9876",
            Tracer = tracer,
            Log = XTrace.Log,
        };
        services.AddSingleton(producer);

        //services.AddHostedService<RocketMqWorker>();
    }
}
