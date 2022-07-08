using NewLife;
using NewLife.Log;
using NewLife.RocketMQ;
using NewLife.RocketMQ.Protocol;

namespace Zero.Worker;

public class RocketMqWorker : IHostedService
{
    private Consumer _consumer;
    private readonly ITracer _tracer;

    public RocketMqWorker(ITracer tracer) => _tracer = tracer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // 引入 RocketMQ 消费者
        var consumer = new Consumer
        {
            Topic = "nx_test",
            Group = "test",
            NameServerAddress = "127.0.0.1:9876",

            FromLastOffset = true,
            BatchSize = 20,

            Tracer = _tracer,
            Log = XTrace.Log,

            OnConsume = OnConsume
        };
        consumer.Start();

        _consumer = consumer;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Stop();
        _consumer.TryDispose();

        return Task.CompletedTask;
    }

    private Boolean OnConsume(MessageQueue queue, MessageExt[] messages)
    {
        XTrace.WriteLine("[{0}@{1}]收到消息[{2}]", queue.BrokerName, queue.QueueId, messages.Length);

        foreach (var item in messages.ToList())
        {
            XTrace.WriteLine($"消息：主键【{item.Keys}】，产生时间【{item.BornTimestamp.ToDateTime()}】，内容【{item.Body.ToStr()}】");
        }

        return true;
    }
}