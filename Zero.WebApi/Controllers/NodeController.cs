using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLife.Remoting;
using NewLife.Remoting.Models;
using Stardust.Models;
using Zero.Data.Nodes;
using Zero.WebApi.Services;

namespace Zero.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NodeController : BaseController
{
    private Node _node;
    private readonly NodeService _nodeService;
    private readonly ApiSetting _setting;

    public NodeController(NodeService nodeService, ApiSetting setting)
    {
        _nodeService = nodeService;
        _setting = setting;
    }

    #region 令牌验证
    protected override Boolean OnAuthorize(String token)
    {
        _node = _nodeService.DecodeToken(token, _setting.TokenSecret);
        return _node != null;
    }

    protected override void OnWriteError(String action, String message) { }
    #endregion

    #region 登录
    [AllowAnonymous]
    [HttpPost(nameof(Login))]
    public LoginResponse Login(LoginInfo inf)
    {
        var ip = UserHost;
        var code = inf.Code;
        var node = Node.FindByCode(code, true);
        var oldSecret = node?.Secret;
        _node = node;

        if (node != null && !node.Enable) throw new ApiException(99, "禁止登录");

        // 设备不存在或者验证失败，执行注册流程
        if (node == null || !_nodeService.Auth(node, inf, ip, inf.Secret))
            node = _nodeService.Register(inf, ip);

        if (node == null) throw new ApiException(12, "节点鉴权失败");

        var tokenModel = _nodeService.Login(node, inf, ip, _setting);

        var rs = new LoginResponse
        {
            Name = node.Name,
            Token = tokenModel.AccessToken,
        };

        // 动态注册，下发节点证书
        if (node.Code != code || node.Secret != oldSecret)
        {
            rs.Code = node.Code;
            rs.Secret = node.Secret;
        }

        return rs;
    }

    /// <summary>注销</summary>
    /// <param name="reason">注销原因</param>
    /// <returns></returns>
    [HttpGet(nameof(Logout))]
    [HttpPost(nameof(Logout))]
    public LoginResponse Logout(String reason)
    {
        if (_node != null) _nodeService.Logout(_node, reason, UserHost);

        return new LoginResponse
        {
            Name = _node?.Name,
            Token = null,
        };
    }
    #endregion

    #region 心跳
    [HttpPost(nameof(Ping))]
    public PingResponse Ping(PingInfo inf) => _nodeService.Ping(_node, inf, Token, UserHost, _setting);

    [AllowAnonymous]
    [HttpGet(nameof(Ping))]
    public PingResponse Ping() => new() { Time = 0, ServerTime = DateTime.UtcNow.ToLong(), };
    #endregion
}