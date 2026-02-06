using NewLife.Caching;
using NewLife.MQTT;
using NewLife.RocketMQ;
using NewLife.RocketMQ.Protocol;
using Zero.Console.Workers;

namespace Zero.Console;

static class ServiceExtensions
{
    public static void AddRedis(this IObjectContainer services)
    {
        // 引入 Redis，用于消息队列和缓存，单例，带性能跟踪
        var rds = new FullRedis
        {
            Tracer = services.GetService<ITracer>(),
            Log = XTrace.Log,
        };
        rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");

        services.AddSingleton<ICache>(rds);
        services.AddSingleton(rds);

        services.AddHostedService<RedisWorker>();
    }

    public static void AddMqtt(this IObjectContainer services)
    {
        // 引入 MQTT
        var mqtt = new MqttClient
        {
            Server = "tcp://iot.feifan.link:1883",
            UserName = "stone",
            Password = "Pass@word",
            ClientId = Environment.MachineName,

            Tracer = services.GetService<ITracer>(),
            Log = XTrace.Log,
        };
        services.AddSingleton(mqtt);

        services.AddHostedService<MqttWorker>();
    }

    public static void AddRocketMQ(this IObjectContainer services)
    {
        // 引入 RocketMQ 生产者
        var producer = new Producer
        {
            NameServerAddress = "rocketmq.newlifex.com:9876",
            Version = MQVersion.V5_2_0,
            Topic = "nx_test",

            Tracer = services.GetService<ITracer>(),
            Log = XTrace.Log,
        };
        services.AddSingleton(producer);

        services.AddHostedService<RocketMqWorker>();
    }
}
