using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife;
using NewLife.Caching;
using NewLife.Cube;
using NewLife.Cube.WebMiddleware;
using NewLife.Log;
using NewLife.Remoting;
using Stardust.Monitors;
using XCode.DataAccessLayer;

namespace Zero.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var server = Stardust.Setting.Current.Server;
            if (server.IsNullOrEmpty()) server = "http://star.newlifex.com:6600";

            // APM跟踪器
            var tracer = new StarTracer(server) { Log = XTrace.Log };
            DefaultTracer.Instance = tracer;
            ApiHelper.Tracer = tracer;
            DAL.GlobalTracer = tracer;
            TracerMiddleware.Tracer = tracer;

            services.AddSingleton<ITracer>(tracer);

            // 引入Redis，用于消息队列和缓存，单例，带性能跟踪
            var rds = new FullRedis { Tracer = tracer };
            rds.Init("server=127.0.0.1:6379;password=;db=3;timeout=5000");
            services.AddSingleton<ICache>(rds);
            services.AddSingleton<Redis>(rds);
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=CubeHome}/{action=Index}/{id?}");
            });
        }
    }
}
