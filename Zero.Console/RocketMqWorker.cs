using NewLife.RocketMQ;
using NewLife.RocketMQ.Protocol;

namespace Zero.Console
{
    /// <summary>
    /// RocketMQ消费端
    /// </summary>
    public class RocketMqWorker : IHostedService
    {
        private readonly Consumer _consumer;

        public RocketMqWorker(Consumer consumer) => _consumer = consumer;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            _consumer.OnConsume = OnConsume;
            _consumer.Start();
        }

        Boolean OnConsume(MessageQueue queue, MessageExt[] message)
        {
            XTrace.WriteLine("[{0}@{1}]收到消息[{2}]", queue.BrokerName, queue.QueueId, message.Length);

            foreach (var item in message.ToList())
            {
                XTrace.WriteLine($"消息：主键【{item.Keys}】，产生时间【{item.BornTimestamp.ToDateTime()}】，内容【{item.Body.ToStr()}】");
            }

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}