using AntJob;
using NewLife;

namespace Zero.AntJob;

public class AntJobWorker : BackgroundService
{
    private readonly Scheduler _scheduler;

    public AntJobWorker(Scheduler scheduler) => _scheduler = scheduler;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 启动调度引擎，调度器内部多线程处理
        _scheduler.Start();

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _scheduler.TryDispose();

        return base.StopAsync(cancellationToken);
    }
}