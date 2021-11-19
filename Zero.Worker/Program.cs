using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife;
using NewLife.Caching;
using NewLife.Log;
using NewLife.MQTT;
using NewLife.RocketMQ;
using Stardust;

namespace Zero.Worker
{
    public class Program
    {
        public static void Main(String[] args)
        {
            // 启用控制台日志，拦截所有异常
            XTrace.UseConsole();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(String[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        public static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            // 配置星尘。自动读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
            var star = new StarFactory(null, null, null);
            if (star.Server.IsNullOrEmpty()) star = null;

            InitRedis(services, star?.Tracer);
            InitMqtt(services, star?.Tracer);
            InitRocketMq(services, star?.Tracer);

            // 注册后台服务
            services.AddHostedService<Worker>();
            services.AddHostedService<RedisWorker>();
            //services.AddHostedService<MqttWorker>();
            //services.AddHostedService<RocketMqWorker>();
        }

        private static void InitRedis(IServiceCollection services, ITracer tracer)
        {
            // 引入 Redis，用于消息队列和缓存，单例，带性能跟踪
            var rds = new FullRedis { Tracer = tracer };
            rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
            services.AddSingleton<ICache>(rds);
            services.AddSingleton(rds);
        }

        private static void InitMqtt(IServiceCollection services, ITracer tracer)
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

        private static void InitRocketMq(IServiceCollection services, ITracer tracer)
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
                BatchSize = 20,

                Tracer = tracer,
                Log = XTrace.Log,
            };
            services.AddSingleton(consumer);
        }
    }
}