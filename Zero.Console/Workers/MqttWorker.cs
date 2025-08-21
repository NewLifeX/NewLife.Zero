using NewLife.Data;
using NewLife.MQTT;
using NewLife.MQTT.Messaging;

namespace Zero.Console.Workers;

/// <summary>MQTT消费端</summary>
public class MqttWorker(MqttClient mqtt) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("MqttWorker.StartAsync");

        var task = ExecuteAsync(cancellationToken);
        return task.IsCompleted ? task : Task.CompletedTask;
    }

    protected async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        mqtt.Received += OnConsume;

        // 连接
        await mqtt.ConnectAsync(cancellationToken);

        // 订阅
        await mqtt.SubscribeAsync(["mqttTopic", "qosTopic"], QualityOfService.AtMostOnce, cancellationToken);
    }

    void OnConsume(Object sender, EventArgs<PublishMessage> args)
    {
        var pm = args.Arg;
        var msg = pm.Payload.ToStr();

        XTrace.WriteLine("MQTT消费[{0}]：{1}", pm.Topic, msg);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("MqttWorker.StopAsync");

        mqtt.TryDispose();

        return Task.CompletedTask;
    }
}