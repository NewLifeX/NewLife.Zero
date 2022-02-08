using NewLife;
using NewLife.Log;
using NewLife.RocketMQ;

namespace Zero.Worker;

public class RocketMqWorker : BackgroundService
{
    private readonly Consumer _consumer;

    public RocketMqWorker(Consumer consumer) => _consumer = consumer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        _consumer.OnConsume = (q, ms) =>
        {
            XTrace.WriteLine("[{0}@{1}]收到消息[{2}]", q.BrokerName, q.QueueId, ms.Length);

            foreach (var item in ms.ToList())
            {
                XTrace.WriteLine($"消息：主键【{item.Keys}】，产生时间【{item.BornTimestamp.ToDateTime()}】，内容【{item.Body.ToStr()}】");
            }

            return true;
        };

        _consumer.Start();
    }
}