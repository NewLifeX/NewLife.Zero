using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Remoting;
using Stardust.Monitors;
using XCode.DataAccessLayer;

namespace Zero.Worker
{
    public class Program
    {
        public static void Main(String[] args)
        {
            // 启用控制台日志，拦截所有异常
            XTrace.UseConsole();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(String[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        public static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var server = Stardust.Setting.Current.Server;
            if (server.IsNullOrEmpty()) server = "http://star.newlifex.com:6600";

            // APM跟踪器
            var tracer = new StarTracer(server) { Log = XTrace.Log };
            DefaultTracer.Instance = tracer;
            ApiHelper.Tracer = tracer;
            DAL.GlobalTracer = tracer;

            services.AddSingleton<ITracer>(tracer);

            // 引入Redis，用于消息队列和缓存，单例，带性能跟踪
            var rds = new FullRedis { Tracer = tracer };
            rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
            services.AddSingleton<ICache>(rds);
            services.AddSingleton<Redis>(rds);
            services.AddSingleton(rds);

            // 注册后台服务
            services.AddHostedService<Worker>();
        }
    }
}