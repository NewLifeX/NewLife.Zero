using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NewLife.Log;

namespace Zero.Agent2;

/// <summary>后台工作者。可根据业务需要定义多个后台工作者</summary>
public class Worker : BackgroundService
{
    private readonly ILog _logger;

    public Worker(ILog logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Info("Worker running at: {0}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine(nameof(StartAsync));

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine(nameof(StopAsync));

        return base.StopAsync(cancellationToken);
    }
}