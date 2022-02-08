using NewLife.Log;
using NewLife.MQTT;

namespace Zero.Worker;

public class MqttWorker : BackgroundService
{
    private readonly MqttClient _mqtt;

    public MqttWorker(MqttClient mqtt) => _mqtt = mqtt;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

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
}