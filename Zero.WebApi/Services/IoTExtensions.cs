using Microsoft.AspNetCore.WebSockets;
using NewLife.Remoting.Extensions;
using NewLife.Remoting.Extensions.Models;
using NewLife.Remoting.Extensions.Services;
using NewLife.Remoting.Models;
using Zero.Models;

namespace Zero.WebApi.Services;

/// <summary>IoT扩展</summary>
public static class IoTExtensions
{
    /// <summary>添加IoT客户端服务端架构服务，支持客户端登录、心跳、更新以及指令下发等操作</summary>
    /// <remarks>
    /// 注册登录心跳等模型类，可在此扩展模型类，传输更多内容；
    /// 注册IDeviceService服务，提供登录心跳等基础实现；
    /// 注册TokenService令牌服务，提供令牌颁发与验证服务；
    /// 注册密码提供者，用于通信过程中保护密钥，避免明文传输；
    /// 注册缓存提供者的默认实现；
    /// 注册节点在线后台服务，定时检查节点在线状态；
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="setting"></param>
    /// <returns></returns>
    public static IServiceCollection AddIoT(this IServiceCollection services, ITokenSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        // 逐个注册每一个用到的服务，必须做到清晰明了
        services.AddSingleton<IDeviceService, NodeService>();

        // 根据项目需要，可以注册 LoginRequest 和 PingRequest 的扩展实现
        services.AddTransient<ILoginRequest, LoginInfo>();
        services.AddTransient<IPingRequest, PingInfo>();

        // 注册Remoting所必须的服务
        services.AddRemoting(setting);

        // 后台服务
        services.AddHostedService<NodeOnlineService>();

        return services;
    }

    /// <summary>使用IoT客户端服务端架构服务，启用WebSocket</summary>
    /// <param name="app"></param>
    public static void UseIoT(this IApplicationBuilder app)
    {
        //app.UseRemoting();

        // 判断是否已经添加了WebSocket中间件
        if (!app.Properties.TryGetValue("__MiddlewareDescriptions", out var value) ||
            value is not IList<String> result || !result.Contains(typeof(WebSocketMiddleware).FullName!))
        {
            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),
            });
        }
    }
}
