using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife.Caching;
using NewLife.Cube;
using NewLife.Cube.WebMiddleware;
using XCode;

namespace Zero.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
            var star = services.AddStardust(null);

            // APM跟踪器
            TracerMiddleware.Tracer = star.Tracer;

            // 引入Redis，用于消息队列和缓存，单例，带性能跟踪
            var rds = new FullRedis { Tracer = star.Tracer };
            rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
            services.AddSingleton<ICache>(rds);
            services.AddSingleton(rds);

            services.AddControllersWithViews();

            // 引入魔方
            services.AddCube();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 使用Cube前添加自己的管道
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/CubeHome/Error");

            // 预热数据层，执行反向工程建表等操作
            EntityFactory.InitConnection("Membership");
            EntityFactory.InitConnection("Log");
            EntityFactory.InitConnection("Cube");
            EntityFactory.InitConnection("Zero");

            app.UseCube(env);

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            //app.UseRouting();

            //app.UseAuthorization();

            // 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
            app.RegisterService("Zero.Web", null, "dev");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=CubeHome}/{action=Index}/{id?}");
            });
        }
    }
}
