using NewLife.RocketMQ;
using NewLife.RocketMQ.Protocol;

namespace Zero.Console.Workers;

/// <summary>RocketMQ消费端</summary>
public class RocketMqWorker(Producer producer, ITracer tracer) : IHostedService
{
    private Consumer _consumer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("RocketMQ.StartAsync");

        // 引入 RocketMQ 消费者
        var consumer = new Consumer
        {
            Topic = producer.Topic,
            Group = Environment.MachineName,
            NameServerAddress = producer.NameServerAddress,

            FromLastOffset = true,
            BatchSize = 20,

            Tracer = tracer,
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
        XTrace.WriteLine("RocketMQ消费[{0}@{1}][{2}]：", queue.BrokerName, queue.QueueId, messages.Length);

        foreach (var item in messages.ToList())
        {
            XTrace.WriteLine($"\t消息：{item.Body.ToStr()}");
        }

        return true;
    }
}