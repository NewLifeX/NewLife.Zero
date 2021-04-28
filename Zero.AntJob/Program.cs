using System;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Model;
using Stardust;
using XCode.DataAccessLayer;

namespace Zero.AntJob
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            XTrace.UseConsole();

            var services = ObjectContainer.Current;

            var star = new StarFactory(null, null, null);

            // 数据库连接配置
            DAL.SetConfig(star.Config);

            // 配置缓存，使用 MemoryCache 或 Redis，其中Redis从配置中心读取配置信息
            var cache = new MemoryCache { Capacity = 1_000_000 };
            services.AddSingleton<ICache>(cache);
            //services.AddSingleton<ICache>(p => new FullRedis(p, "redisCache"));

            // 后台任务
            var host = services.BuildHost();
            host.Add<JobHost>();
            host.Run();
        }
    }
}