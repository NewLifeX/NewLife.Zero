using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Reflection;

namespace Zero.Web.Controllers;

[ApiFilter]
[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBaseX
{
    private static readonly String _OS = Environment.OSVersion + "";

    /// <summary>服务器信息，用户健康检测</summary>
    /// <param name="state">状态信息</param>
    /// <returns></returns>
    [HttpGet]
    public Object Get(String state)
    {
        var asmx = AssemblyX.Entry;
        var conn = HttpContext.Connection;
        var remote = conn.RemoteIpAddress;
        if (remote.IsIPv4MappedToIPv6) remote = remote.MapToIPv4();
        var ip = HttpContext.GetUserHost();

        var rs = new
        {
            asmx?.Name,
            asmx?.Title,
            asmx?.FileVersion,
            asmx?.Compile,
            OS = _OS,

            UserHost = ip + "",
            Remote = remote + "",
            Port = conn.LocalPort,
            Time = DateTime.Now,
            State = state,
        };

        return rs;
    }
}