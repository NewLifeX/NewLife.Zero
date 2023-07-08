using NewLife.Caching;
using NewLife.MQTT;
using NewLife.RocketMQ;
using Stardust;
using Zero.Console;

//!!! 轻量级控制台项目模板

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

// 初始化对象容器，提供注入能力
var services = ObjectContainer.Current;
services.AddSingleton(XTrace.Log);

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址
var star = new StarFactory();
if (star.Server.IsNullOrEmpty()) star = null;

// 初始化Redis、MQTT、RocketMQ，注册服务到容器
InitRedis(services, star?.Tracer);
InitMqtt(services, star?.Tracer);
InitRocketMq(services, star?.Tracer);

// 注册后台任务 IHostedService
var host = services.BuildHost();
host.Add<Worker>();
host.Add<RedisWorker>();
//host.Add<RocketMqWorker>();
//host.Add<MqttWorker>();

// 异步阻塞，友好退出
await host.RunAsync();



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
        Tracer = tracer,
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
}