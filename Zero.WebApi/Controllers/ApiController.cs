using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using NewLife;
using NewLife.Data;
using NewLife.Reflection;
using NewLife.Serialization;

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
        private static AssemblyX _entry = AssemblyX.Entry;
        private static AssemblyX _executing = AssemblyX.Create(Assembly.GetExecutingAssembly());

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

        private static Packet _myInfo;
        /// <summary>服务器信息，用户健康检测，二进制压测</summary>
        /// <param name="state">状态信息</param>
        /// <returns></returns>
        [HttpPost(nameof(Info2))]
        public async Task<ObjectResult> Info2()
        {
            if (_myInfo == null)
            {
                // 不包含时间和远程地址
                var rs = new
                {
                    MachineNam = _MachineName,
                    UserName = _UserName,
                    LocalIP = _LocalIP,
                };
                _myInfo = new Packet(rs.ToJson().GetBytes());
            }

            var buf = new Byte[4096];
            var count = await Request.Body.ReadAsync(buf, 0, buf.Length);
            var state = new Packet(buf, 0, count);

            var pk = _myInfo.Slice(0, -1);
            pk.Append(state);

            var res = new ObjectResult(pk.GetStream());
            res.Formatters.Add(new StreamOutputFormatter());
            res.ContentTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));

            return res;
        }
    }
}