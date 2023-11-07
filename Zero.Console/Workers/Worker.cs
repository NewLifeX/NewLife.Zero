using NewLife.Caching;
using NewLife.MQTT;
using NewLife.Remoting;
using NewLife.RocketMQ;
using NewLife.Serialization;
using Stardust;
using Zero.Console.Models;

namespace Zero.Console.Workers;

/// <summary>
/// 后台任务。支持构造函数注入服务
/// </summary>
public class Worker : IHostedService
{
    private readonly ILog _logger;
    private readonly FullRedis _redis;
    private readonly MqttClient _mqtt;
    private readonly Producer _producer;
    private readonly StarFactory _star;
    private ApiHttpClient _client;

    public Worker(ILog logger, FullRedis redis, MqttClient mqtt, Producer producer, StarFactory star)
    {
        _logger = logger;
        _redis = redis;
        _mqtt = mqtt;
        _producer = producer;
        _star = star;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("Worker.StartAsync");

        // 从注册中心消费一个服务，创建客户端，该客户端能够自动感知服务提供者的地址变化
        _client = _star.CreateForService("Zero.WebApi", "dev") as ApiHttpClient;

        var task = ExecuteAsync(cancellationToken);
        return task.IsCompleted ? task : Task.CompletedTask;
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_client.Services.Count == 0) return;

        // Redis 可信消息队列，支持消费确认
        var rdsQueue = _redis.GetReliableQueue<Object>("rdsTopic");

        //// RocketMQ 生产者
        //_producer.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            var area = new Area { Code = 110000, Name = "北京市" };

            // Redis 发布
            rdsQueue.Add(area);

            //// MQTT 发布
            //await _mqtt.PublicAsync("mqttTopic", area);

            //// RocketMQ 发布
            //_producer.Publish(area);

            // 调用远程服务
            _client?.Get<Object>("api", new { state = area.ToJson() });

            _logger.Info("Worker running at: {0}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}