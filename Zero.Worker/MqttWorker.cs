using NewLife.Data;
using NewLife.Log;
using NewLife.MQTT;

namespace Zero.Worker;

public class MqttWorker(MqttClient mqtt) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        mqtt.Received += (s, e) =>
        {
            var pm = e.Arg;
            var msg = pm.Payload.ToStr();

            XTrace.WriteLine("消费消息：[{0}] {1}", pm.Topic, msg);
        };

        // 连接
        await mqtt.ConnectAsync();

        // 订阅
        await mqtt.SubscribeAsync(new[] { "mqttTopic", "qosTopic" });
    }
}