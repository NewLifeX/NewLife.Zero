using NewLife.Remoting.Extensions;
using NewLife.Remoting.Extensions.Models;
using NewLife.Remoting.Extensions.Services;
using NewLife.Remoting.Models;
using Zero.Models;

namespace Zero.WebApi.Services;

/// <summary>IoT扩展</summary>
public static class IoTExtensions
{
    public static IServiceCollection AddIoT(this IServiceCollection services, ITokenSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        // 逐个注册每一个用到的服务，必须做到清晰明了
        services.AddSingleton<IDeviceService, NodeService>();

        services.AddTransient<ILoginRequest, LoginInfo>();
        services.AddTransient<IPingRequest, PingInfo>();

        // 注册Remoting所必须的服务
        services.AddRemoting(setting);

        // 后台服务
        services.AddHostedService<NodeOnlineService>();

        return services;
    }
}
