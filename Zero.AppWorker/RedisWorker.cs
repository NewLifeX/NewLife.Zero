using System;
using System.Threading;
using System.Threading.Tasks;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Model;
using Zero.AppWorker.Models;

namespace Zero.AppWorker
{
    public class RedisWorker : IHostedService
    {
        private readonly FullRedis _redis;
        private readonly ILog _log;
        private RedisReliableQueue<String> _queue;

        public RedisWorker(FullRedis redis, ILog log)
        {
            _redis = redis;
            _log = log;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var task = ExecuteAsync(cancellationToken);
            return task.IsCompleted ? task : Task.CompletedTask;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            // Redis 可信消息队列，支持消费确认
            _queue = _redis.GetReliableQueue<String>("rdsTopic");

            await _queue.ConsumeAsync<Area>(area =>
            {
                XTrace.WriteLine("RedisQueue.Consume {0} {1}", area.Code, area.Name);
            }, stoppingToken, _log);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}