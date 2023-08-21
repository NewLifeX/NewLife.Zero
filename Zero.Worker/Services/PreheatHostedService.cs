using NewLife.Caching;
using NewLife.Log;
using XCode.Membership;
using Zero.Data.Nodes;

namespace Zero.Worker.Services;

/// <summary>预热服务。在提供服务前预热数据库和中间件的网络连接，避免应用启动时的网络连接冲击导致雪崩</summary>
public class PreheatHostedService : IHostedService
{
    private readonly ICacheProvider _cacheProvider;

    public PreheatHostedService(ICacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("在提供服务前预热数据库和中间件的网络连接，避免应用启动时的网络连接冲击导致雪崩……");

        // 预热数据库连接
        XTrace.WriteLine("节点数：{0:n0}", Node.Meta.Count);
        XTrace.WriteLine("用户数：{0:n0}", User.Meta.Count);

        // 预热缓存
        var cache = _cacheProvider.Cache;
        if (cache != null)
            XTrace.WriteLine("缓存总数：{0:n0}", cache.Count);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}