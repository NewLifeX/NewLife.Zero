using NewLife.Caching;
using Zero.Worker.Models;

namespace Zero.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FullRedis _redis;
    //private readonly MqttClient _mqtt;
    //private readonly Producer _producer;

    public Worker(ILogger<Worker> logger, FullRedis redis/*, MqttClient mqtt, Producer producer*/)
    {
        _logger = logger;
        _redis = redis;
        //_mqtt = mqtt;
        //_producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        // Redis 可信消息队列，支持消费确认
        var rdsQueue = _redis.GetReliableQueue<Object>("rdsTopic");

        //// RocketMQ 生产者
        //_producer.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            // Redis 发布
            rdsQueue.Add(new Area { Code = 110000, Name = "北京市" });

            //// MQTT 发布
            //await _mqtt.PublicAsync("mqttTopic", new Area { Code = 110000, Name = "北京市" });

            //// RocketMQ 发布
            //_producer.Publish(new Area { Code = 110000, Name = "北京市" });

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}