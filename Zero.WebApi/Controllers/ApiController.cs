using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Reflection;

namespace Zero.WebApi.Controllers
{
    /// <summary>接口探针</summary>
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        /// <summary>获取所有接口</summary>
        /// <returns></returns>
        [HttpGet]
        public Object Get() => Info(null);

        private static readonly String _OS = Environment.OSVersion + "";
        private static readonly String _MachineName = Environment.MachineName;
        private static readonly String _UserName = Environment.UserName;
        private static readonly String _LocalIP = NetHelper.MyIP() + "";
        private static readonly AssemblyX _entry = AssemblyX.Entry;
        private static readonly AssemblyX _executing = AssemblyX.Create(Assembly.GetExecutingAssembly());

        /// <summary>服务器信息，用户健康检测</summary>
        /// <param name="state">状态信息</param>
        /// <returns></returns>
        [HttpGet(nameof(Info))]
        public Object Info(String state)
        {
            var conn = HttpContext.Connection;

            var ip = Request.Host;

            var rs = new
            {
                Server = _entry?.Name,
                _entry?.Version,
                OS = _OS,
                MachineName = _MachineName,
                UserName = _UserName,
                ApiVersion = _executing?.Version,

                LocalIP = _LocalIP,
                conn.LocalPort,
                Remote = ip + "",
                Time = DateTime.Now,
                State = state,
            };

            return rs;
        }
    }
}