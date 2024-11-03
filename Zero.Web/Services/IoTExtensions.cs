using NewLife.Remoting.Extensions;
using NewLife.Remoting.Extensions.Models;
using NewLife.Remoting.Extensions.Services;
using NewLife.Remoting.Models;
using Zero.Models;

namespace Zero.Web.Services;

/// <summary>IoT扩展</summary>
public static class IoTExtensions
{
    /// <summary>添加IoT客户端服务端架构服务，支持客户端登录、心跳、更新以及指令下发等操作</summary>
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
}
