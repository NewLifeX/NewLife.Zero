using NewLife.Caching;
using NewLife.MQTT;
using NewLife.Remoting;
using NewLife.RocketMQ;
using NewLife.Serialization;
using Stardust;
using Zero.Console.Models;

namespace Zero.Console.Workers;

/// <summary>后台任务。支持构造函数注入服务</summary>
public class Worker(ILog logger, FullRedis redis, StarFactory star, IServiceProvider serviceProvider) : IHostedService
{
    private ApiHttpClient _client;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("Worker.StartAsync");

        try
        {
            // 从注册中心消费一个服务，创建客户端，该客户端能够自动感知服务提供者的地址变化
            _client = star.CreateForService("Zero.WebApi", "dev") as ApiHttpClient;
        }
        catch (Exception ex)
        {
            XTrace.Log.Error(ex.Message);
        }

        _ = ExecuteAsync(cancellationToken);

        return Task.CompletedTask;
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        // Redis 可信消息队列，支持消费确认
        var rdsQueue = redis.GetStream<Object>("rdsTopic");

        // MQTT 客户端
        var mqtt = serviceProvider.GetService<MqttClient>();

        // RocketMQ 生产者
        var producer = serviceProvider.GetService<Producer>();
        producer?.Start();

        var index = 1;
        while (!stoppingToken.IsCancellationRequested)
        {
            var area = new Area { Code = 110000, Name = "北京市" + index };
            XTrace.WriteLine("");
            XTrace.WriteLine("[{0}]正在向多个队列发布消息……", index++);

            // Redis 发布
            rdsQueue.Add(area);

            // MQTT 发布
            if (mqtt != null) await mqtt?.PublishAsync("mqttTopic", area);

            // RocketMQ 发布
            producer?.Publish(area);

            // 调用远程服务
            if (_client != null && _client.Services.Count > 0)
                await _client.GetAsync<Object>("api", new { state = area.ToJson() });

            logger.Info("Worker running at: {0}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("Worker.StopAsync");

        return Task.CompletedTask;
    }
}