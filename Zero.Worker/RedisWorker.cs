using NewLife.Caching;
using NewLife.Log;
using Zero.Worker.Models;
using NewLife.Caching.Queues;

namespace Zero.Worker;

public class RedisWorker(FullRedis redis) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        // Redis 可信消息队列，支持消费确认
        var rdsQueue = redis.GetReliableQueue<String>("rdsTopic");

        await rdsQueue.ConsumeAsync<Area>(area =>
        {
            XTrace.WriteLine("RedisQueue.Consume {0} {1}", area.Code, area.Name);
        }, stoppingToken, XTrace.Log);
    }
}