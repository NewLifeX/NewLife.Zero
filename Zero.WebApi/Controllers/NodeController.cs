using Microsoft.AspNetCore.Mvc;
using NewLife.Log;
using NewLife.Remoting.Extensions;
using NewLife.Remoting.Models;
using NewLife.Remoting.Services;
using Zero.Data.Nodes;
using Zero.WebApi.Services;

namespace Zero.WebApi.Controllers;

/// <summary>设备控制器</summary>
[ApiFilter]
[ApiController]
[Route("[controller]")]
public class NodeController : BaseDeviceController
{
    /// <summary>当前设备</summary>
    public Node Node { get; set; }

    private readonly NodeService _nodeService;
    private readonly ITracer _tracer;

    #region 构造
    /// <summary>实例化设备控制器</summary>
    /// <param name="serviceProvider"></param>
    /// <param name="tracer"></param>
    public NodeController(IServiceProvider serviceProvider, ITracer tracer) : base(serviceProvider)
    {
        _nodeService = serviceProvider.GetRequiredService<IDeviceService>() as NodeService;
        _tracer = tracer;
    }

    protected override Boolean OnAuthorize(String token)
    {
        if (!base.OnAuthorize(token)) return false;

        Node = _device as Node;

        return true;
    }
    #endregion

    /// <summary>心跳</summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override IPingResponse OnPing(IPingRequest request)
    {
        var rs = base.OnPing(request);

        var node = Node;
        if (node != null)
        {
            rs.Period = node.Period;

            if (rs is PingResponse rs2)
            {
                rs2.NewServer = node.NewServer;
            }
        }

        return rs;
    }
}