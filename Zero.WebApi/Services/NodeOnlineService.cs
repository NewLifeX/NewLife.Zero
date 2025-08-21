using NewLife;
using NewLife.Log;
using NewLife.Remoting.Models;
using NewLife.Remoting.Services;
using NewLife.Threading;
using Zero.Data.Nodes;

namespace Zero.WebApi.Services;

/// <summary>节点在线服务</summary>
/// <remarks>
/// 实例化节点在线服务
/// </remarks>
/// <param name="nodeService"></param>
/// <param name="setting"></param>
/// <param name="tracer"></param>
public class NodeOnlineService(IDeviceService nodeService, ITokenSetting setting, ITracer tracer) : IHostedService
{
    private TimerX _timer;

    #region 方法
    /// <summary>
    /// 开始服务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new TimerX(CheckOnline, null, 5_000, 30_000) { Async = true };

        return Task.CompletedTask;
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.TryDispose();

        return Task.CompletedTask;
    }

    private void CheckOnline(Object state)
    {
        // 节点超时
        if (setting.SessionTimeout > 0)
        {
            using var span = tracer?.NewSpan(nameof(CheckOnline));

            var rs = NodeOnline.ClearExpire(TimeSpan.FromSeconds(setting.SessionTimeout));
            if (rs != null)
                foreach (var olt in rs)
                {
                    var node = olt?.Node;
                    var msg = $"[{node}]登录于{olt.CreateTime.ToFullString()}，最后活跃于{olt.UpdateTime.ToFullString()}";
                    nodeService.WriteHistory(node, "超时下线", true, msg, olt.CreateIP);

                    if (nodeService is NodeService ds)
                        ds.RemoveOnline(olt.NodeId, olt.CreateIP);

                    if (node != null)
                    {
                        // 计算在线时长
                        if (olt.CreateTime.Year > 2000 && olt.UpdateTime.Year > 2000)
                        {
                            node.OnlineTime += (Int32)(olt.UpdateTime - olt.CreateTime).TotalSeconds;
                            node.Update();
                        }

                        CheckOffline(node, "超时下线");
                    }
                }
        }
    }

    /// <summary>
    /// 检查离线
    /// </summary>
    /// <param name="node"></param>
    /// <param name="reason"></param>
    public static void CheckOffline(Node node, String reason)
    {
        //todo 下线告警
    }
    #endregion
}