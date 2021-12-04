using NewLife.Caching;
using NewLife.MQTT;
using NewLife.RocketMQ;
using Zero.Console.Models;

namespace Zero.Console
{
    /// <summary>
    /// 后台任务。支持构造函数注入服务
    /// </summary>
    public class Worker : IHostedService
    {
        private readonly ILog _logger;
        private readonly FullRedis _redis;
        private readonly MqttClient _mqtt;
        private readonly Producer _producer;

        public Worker(ILog logger, FullRedis redis, MqttClient mqtt, Producer producer)
        {
            _logger = logger;
            _redis = redis;
            _mqtt = mqtt;
            _producer = producer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var task = ExecuteAsync(cancellationToken);
            return task.IsCompleted ? task : Task.CompletedTask;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Redis 可信消息队列，支持消费确认
            var rdsQueue = _redis.GetReliableQueue<Object>("rdsTopic");

            //// RocketMQ 生产者
            //_producer.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                // Redis 发布
                rdsQueue.Add(new Area { Code = 110000, Name = "北京市" });

                //// MQTT 发布
                //await _mqtt.PublicAsync("mqttTopic", new Area { Code = 110000, Name = "北京市" });

                //// RocketMQ 发布
                //_producer.Publish(new Area { Code = 110000, Name = "北京市" });

                _logger.Info("Worker running at: {0}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}