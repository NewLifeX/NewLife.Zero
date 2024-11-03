using NewLife;
using NewLife.Log;
using NewLife.Model;
using ZeroClient;

namespace Zero.Client;

/// <summary>
/// 后台任务。支持构造函数注入服务
/// </summary>
public class Worker : IHostedService
{
    private NodeClient _client;
    private readonly ILog _log;
    private readonly ITracer _tracer;

    public Worker(ILog log, ITracer tracer)
    {
        _log = log;
        _tracer = tracer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        XTrace.WriteLine("开始Node客户端");

#if DEBUG
        // 降低日志等级，输出通信详情。生产环境不建议这么做
        XTrace.Log.Level = NewLife.Log.LogLevel.Debug;
#endif

        var set = ClientSetting.Current;

        // 产品编码、产品密钥从IoT管理平台获取，设备编码支持自动注册
        var client = new NodeClient(set)
        {
            Tracer = _tracer,
            Log = _log,
        };

        client.Open();

        _client = client;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _client.TryDispose();

        return Task.CompletedTask;
    }
}