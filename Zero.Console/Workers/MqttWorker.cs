using NewLife.MQTT;
using NewLife.MQTT.Messaging;

namespace Zero.Console.Workers;

/// <summary>
/// MQTT消费端
/// </summary>
public class MqttWorker : IHostedService
{
    private readonly MqttClient _mqtt;

    public MqttWorker(MqttClient mqtt) => _mqtt = mqtt;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("MqttWorker.StartAsync");

        var task = ExecuteAsync(cancellationToken);
        return task.IsCompleted ? task : Task.CompletedTask;
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _mqtt.Received += OnConsume;

        // 连接
        await _mqtt.ConnectAsync();

        // 订阅
        await _mqtt.SubscribeAsync(new[] { "mqttTopic", "qosTopic" });
    }

    void OnConsume(Object sender, EventArgs<PublishMessage> args)
    {
        var pm = args.Arg;
        var msg = pm.Payload.ToStr();

        XTrace.WriteLine("消费消息：[{0}] {1}", pm.Topic, msg);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}