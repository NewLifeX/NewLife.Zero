using System;
using NewLife;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Model;
using NewLife.MQTT;
using NewLife.Remoting;
using NewLife.RocketMQ;
using Stardust.Monitors;
using XCode.DataAccessLayer;

namespace Zero.AppWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            // 启用控制台日志，拦截所有异常
            XTrace.UseConsole();

            var services = ObjectContainer.Current;
            services.AddSingleton(XTrace.Log);

            // 初始化星尘跟踪器
            var tracer = InitTracer(services);

            InitRedis(services, tracer);
            InitMqtt(services, tracer);
            InitRocketMq(services, tracer);

            var host = services.BuildHost();
            host.Add<Worker>();
            host.Add<RedisWorker>();
            //host.Add<RocketMqWorker>();
            //host.Add<MqttWorker>();

            host.Run();
            //await host.RunAsync();
        }

        static ITracer InitTracer(IObjectContainer services)
        {
            var server = Stardust.Setting.Current.Server;
            if (server.IsNullOrEmpty()) server = "http://star.newlifex.com:6600";

            // 星尘监控，性能跟踪器
            var tracer = new StarTracer(server) { Log = XTrace.Log };
            DefaultTracer.Instance = tracer;
            ApiHelper.Tracer = tracer;
            DAL.GlobalTracer = tracer;

            services.AddSingleton<ITracer>(tracer);

            return tracer;
        }

        static void InitRedis(IObjectContainer services, ITracer tracer)
        {
            // 引入 Redis，用于消息队列和缓存，单例，带性能跟踪
            var rds = new FullRedis { Tracer = tracer };
            rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
            services.AddSingleton<ICache>(rds);
            services.AddSingleton(rds);
        }

        static void InitMqtt(IObjectContainer services, ITracer tracer)
        {
            // 引入 MQTT
            var mqtt = new MqttClient
            {
                //Tracer = tracer,
                Log = XTrace.Log,

                Server = "tcp://127.0.0.1:1883",
                ClientId = Environment.MachineName,
                UserName = "stone",
                Password = "Pass@word",
            };
            services.AddSingleton(mqtt);
        }

        static void InitRocketMq(IObjectContainer services, ITracer tracer)
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

            // 引入 RocketMQ 消费者
            var consumer = new Consumer
            {
                Topic = "nx_test",
                Group = "test",
                NameServerAddress = "127.0.0.1:9876",

                FromLastOffset = true,
                SkipOverStoredMsgCount = 0,
                BatchSize = 20,

                Tracer = tracer,
                Log = XTrace.Log,
            };
            services.AddSingleton(consumer);
        }
    }
}