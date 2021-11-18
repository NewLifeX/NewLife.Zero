using System.Threading;
using System.Threading.Tasks;
using NewLife.Log;
using NewLife.Model;
using NewLife.MQTT;

namespace Zero.Console
{
    public class MqttWorker : IHostedService
    {
        private readonly MqttClient _mqtt;

        public MqttWorker(MqttClient mqtt) => _mqtt = mqtt;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var task = ExecuteAsync(cancellationToken);
            return task.IsCompleted ? task : Task.CompletedTask;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mqtt.Received += (s, e) =>
            {
                var pm = e.Arg;
                var msg = pm.Payload.ToStr();

                XTrace.WriteLine("消费消息：[{0}] {1}", pm.Topic, msg);
            };

            // 连接
            await _mqtt.ConnectAsync();

            // 订阅
            await _mqtt.SubscribeAsync(new[] { "mqttTopic", "qosTopic" });
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}