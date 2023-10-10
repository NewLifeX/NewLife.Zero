using NewLife.Log;
using NewLife.Model;
using NewLife.MQTT;
using Stardust;

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

// 初始化对象容器，提供注入能力
var services = ObjectContainer.Current;
services.AddSingleton(XTrace.Log);

// 配置星尘。自动读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust();

// 初始化Redis、MQTT、RocketMQ，注册服务到容器
InitMqtt(services, star?.Tracer);

// 异步阻塞，友好退出
var host = services.BuildHost();
await host.RunAsync();

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
