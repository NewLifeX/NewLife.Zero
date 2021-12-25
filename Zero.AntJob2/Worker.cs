using System.Threading;
using System.Threading.Tasks;
using AntJob;
using Microsoft.Extensions.Hosting;
using NewLife;

namespace Zero.AntJob2
{
    public class Worker : BackgroundService
    {
        private readonly Scheduler _scheduler;

        public Worker(Scheduler scheduler)
        {
            _scheduler = scheduler;
        }

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
}
