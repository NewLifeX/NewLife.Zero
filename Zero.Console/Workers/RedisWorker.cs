using NewLife.Caching;
using NewLife.Caching.Queues;
using Zero.Console.Models;

namespace Zero.Console.Workers;

/// <summary>
/// Redis队列消费端
/// </summary>
public class RedisWorker : IHostedService
{
    private readonly FullRedis _redis;
    private readonly ILog _log;
    private RedisReliableQueue<String> _queue;

    public RedisWorker(FullRedis redis, ILog log)
    {
        _redis = redis;
        _log = log;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("RedisWorker.StartAsync");

        var task = ExecuteAsync(cancellationToken);
        return task.IsCompleted ? task : Task.CompletedTask;
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        // Redis 可信消息队列，支持消费确认
        _queue = _redis.GetReliableQueue<String>("rdsTopic");

        await _queue.ConsumeAsync<Area>(OnConsume, stoppingToken, _log);
    }

    void OnConsume(Area area)
    {
        XTrace.WriteLine("RedisQueue.Consume {0} {1}", area.Code, area.Name);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}