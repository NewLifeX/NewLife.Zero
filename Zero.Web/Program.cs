using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NewLife.Log;
using Zero.Data.Projects;

namespace Zero.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 启用控制台日志，拦截所有异常
            XTrace.UseConsole();

            // 异步初始化
            Task.Run(InitAsync);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void InitAsync()
        {
            // 配置
            var set = NewLife.Setting.Current;
            if (set.IsNew)
            {
                set.DataPath = "../Data";
                set.Save();
            }

            // 初始化数据库
            var n = Team.Meta.Count;
        }
    }
}
