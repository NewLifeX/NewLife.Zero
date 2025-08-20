using NewLife.Caching;
using NewLife.Caching.Queues;
using Zero.Console.Models;

namespace Zero.Console.Workers;

/// <summary>Redis队列消费端</summary>
public class RedisWorker(FullRedis redis) : IHostedService
{
    private RedisStream<Area> _queue;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("RedisQueue.StartAsync");

        // Redis 可信消息队列，支持消费确认
        _queue = redis.GetStream<Area>("rdsTopic");
        _queue.Group = Environment.MachineName; // 设置消费者组，默认是机器名

        _ = _queue.ConsumeAsync(OnConsume, cancellationToken);

        return Task.CompletedTask;
    }

    private Task OnConsume(Area area, Message message, CancellationToken cancellationToken)
    {
        XTrace.WriteLine("Redis消费 {0} {1}", area.Code, area.Name);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("RedisQueue.StopAsync");

        _queue.TryDispose();

        return Task.CompletedTask;
    }
}