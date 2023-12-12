using NewLife.Caching;
using NewLife.Caching.Services;
using NewLife.Cube;
using NewLife.Log;
using XCode;
using Zero.Web.Services;

//!!! 标准Web项目模板，新生命团队强烈推荐

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// 初始化配置文件
InitConfig();

// 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust(null);

// 默认内存缓存，如有配置RedisCache可使用Redis缓存
services.AddSingleton<ICacheProvider, RedisCacheProvider>();

// 引入Redis，用于消息队列和缓存，单例，带性能跟踪。一般使用上面的ICacheProvider替代
//services.AddRedis("127.0.0.1:6379", "123456", 3, 5000);

// 启用接口响应压缩
services.AddResponseCompression();

services.AddControllersWithViews();

// 引入魔方
services.AddCube();

// 后台服务
services.AddHostedService<MyHostedService>();
// 先预热数据，再启动Web服务，避免网络连接冲击
services.AddHostedService<PreheatHostedService>();

var app = builder.Build();

// 使用Cube前添加自己的管道
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/CubeHome/Error");

app.UseResponseCompression();

// 使用魔方
app.UseCube(app.Environment);

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CubeHome}/{action=Index}/{id?}"
);

// 注册退出事件
if (app is IHost host)
    NewLife.Model.Host.RegisterExit(() => host.StopAsync().Wait());

// 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
app.RegisterService("Zero.Web", null, app.Environment.EnvironmentName);

app.Run();


static void InitConfig()
{
    // 配置
    var set = NewLife.Setting.Current;
    if (set.IsNew)
    {
        set.LogPath = "../LogWeb";
        set.DataPath = "../Data";
        set.BackupPath = "../Backup";
        set.Save();
    }
    //var set2 = XCode.Setting.Current;
    //if (set2.IsNew)
    //{
    //    // 关闭SQL日志输出
    //    set2.ShowSQL = false;
    //    set2.Save();
    //}
}
