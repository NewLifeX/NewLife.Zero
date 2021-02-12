using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using NewLife.RocketMQ;

namespace Zero.AppWorker
{
    public class RocketMqWorker : IHostedService
    {
        private readonly Consumer _consumer;

        public RocketMqWorker(Consumer consumer) => _consumer = consumer;

        public async Task StartAsync(CancellationToken cancellationToken)
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

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}