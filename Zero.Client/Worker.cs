using NewLife.Log;
using NewLife.Model;
using NewLife.MQTT;
using NewLife.Remoting;
using NewLife.Serialization;
using Stardust;
using ZeroClient;

namespace Zero.Client;

/// <summary>
/// 后台任务。支持构造函数注入服务
/// </summary>
public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("Worker.StartAsync");

        var task = ExecuteAsync(cancellationToken);
        return task.IsCompleted ? task : Task.CompletedTask;
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000);

        _ = Task.Run(() => ClientTest.Process(_serviceProvider));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}