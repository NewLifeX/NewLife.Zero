using System.Reflection;
using NewLife;
using NewLife.Caching;
using NewLife.Remoting;
using NewLife.Remoting.Models;
using NewLife.Security;
using NewLife.Serialization;
using NewLife.Web;
using Stardust.Models;
using Zero.Data.Nodes;

namespace Zero.WebApi.Services;

public class NodeService
{
    private readonly ICacheProvider _cacheProvider;

    public NodeService(ICacheProvider cacheProvider) => _cacheProvider = cacheProvider;

    #region 注册&登录
    public Boolean Auth(Node node, LoginInfo inf, String ip, String secret)
    {
        if (node == null) return false;

        node = CheckNode(node, inf.Node, inf.ProductCode, ip);
        if (node == null) return false;

        if (node.Secret.IsNullOrEmpty()) return true;
        return !secret.IsNullOrEmpty() && (node.Secret == secret || node.Secret.MD5() == secret);
    }

    public Node Register(LoginInfo inf, String ip)
    {
        var code = inf.Code;
        var secret = inf.Secret;

        var node = Node.FindByCode(code, true);

        // 校验唯一编码，防止客户端拷贝配置
        var autoReg = false;
        if (node == null)
            node = AutoRegister(null, inf, ip, out autoReg);
        else if (node.Secret.IsNullOrEmpty() || secret.IsNullOrEmpty())
            // 登录密码未设置或者未提交，则执行动态注册
            node = AutoRegister(node, inf, ip, out autoReg);
        else if (node.Secret.MD5() != secret)
            node = AutoRegister(node, inf, ip, out autoReg);

        return node;
    }

    public TokenModel Login(Node node, LoginInfo inf, String ip, ApiSetting set)
    {
        if (!inf.ProductCode.IsNullOrEmpty()) node.ProductCode = inf.ProductCode;

        node.UpdateIP = ip;
        //node.FixNameByRule();
        node.Login(inf.Node, ip);

        // 设置令牌
        var tokenModel = IssueToken(node.Code, set);

        // 在线记录
        var olt = GetOnline(node, ip) ?? CreateOnline(node, tokenModel.AccessToken, ip);
        olt.Save(inf.Node, null, tokenModel.AccessToken, ip);

        // 登录历史
        node.WriteHistory("节点鉴权", true, $"[{node.Name}/{node.Code}]鉴权成功 " + inf.ToJson(false, false, false), ip);

        return tokenModel;
    }

    /// <summary>注销</summary>
    /// <param name="reason">注销原因</param>
    /// <param name="ip">IP地址</param>
    /// <returns></returns>
    public NodeOnline Logout(Node node, String reason, String ip)
    {
        var online = GetOnline(node, ip);
        if (online == null) return null;

        var msg = $"{reason} [{node}]]登录于{online.CreateTime}，最后活跃于{online.UpdateTime}";
        node.WriteHistory("节点下线", true, msg, ip);
        online.Delete();

        var sid = $"{node.ID}@{ip}";
        _cacheProvider.Cache.Remove($"NodeOnline:{sid}");

        // 计算在线时长
        if (online.CreateTime.Year > 2000)
        {
            node.OnlineTime += (Int32)(DateTime.Now - online.CreateTime).TotalSeconds;
            node.SaveAsync();
        }

        //NodeOnlineService.CheckOffline(node, "注销");

        return online;
    }

    /// <summary>
    /// 校验节点密钥
    /// </summary>
    /// <param name="node"></param>
    /// <param name="ps"></param>
    /// <returns></returns>
    private Node CheckNode(Node node, NodeInfo di, String productCode, String ip)
    {
        // 校验唯一编码，防止客户端拷贝配置
        var uuid = di.UUID;
        var guid = di.MachineGuid;
        var diskid = di.DiskID;
        if (!uuid.IsNullOrEmpty() && uuid != node.Uuid)
        {
            node.WriteHistory("登录校验", false, $"唯一标识不符！{uuid}!={node.Uuid}", ip);
            return null;
        }
        if (!guid.IsNullOrEmpty() && guid != node.MachineGuid)
        {
            node.WriteHistory("登录校验", false, $"机器标识不符！{guid}!={node.MachineGuid}", ip);
            return null;
        }
        if (!diskid.IsNullOrEmpty() && diskid != node.DiskID)
        {
            node.WriteHistory("登录校验", false, $"磁盘序列号不符！{diskid}!={node.DiskID}", ip);
            return null;
        }
        if (!node.ProductCode.IsNullOrEmpty() && !productCode.IsNullOrEmpty() && !node.ProductCode.EqualIgnoreCase(productCode))
        {
            node.WriteHistory("登录校验", false, $"产品编码不符！{productCode}!={node.ProductCode}", ip);
            return null;
        }

        // 机器名
        if (di.MachineName != node.MachineName)
            node.WriteHistory("登录校验", false, $"机器名不符！{di.MachineName}!={node.MachineName}", ip);

        // 网卡地址
        if (di.Macs != node.MACs)
        {
            var dims = di.Macs?.Split(",") ?? new String[0];
            var nodems = node.MACs?.Split(",") ?? new String[0];
            // 任意网卡匹配则通过
            if (!nodems.Any(e => dims.Contains(e)))
                node.WriteHistory("登录校验", false, $"网卡地址不符！{di.Macs}!={node.MACs}", ip);
        }

        return node;
    }

    private Node AutoRegister(Node node, LoginInfo inf, String ip, out Boolean autoReg)
    {
        //var set = Setting.Current;
        //if (!set.AutoRegister) throw new ApiException(12, "禁止自动注册");

        //// 检查白名单
        ////var ip = UserHost;
        //if (!IsMatchWhiteIP(set.WhiteIP, ip)) throw new ApiException(13, "非法来源，禁止注册");

        var di = inf.Node;
        var code = (di.UUID + di.MachineGuid).GetBytes().Crc().GetBytes().ToHex();
        if (code.IsNullOrEmpty()) Rand.NextString(8);
        node ??= Node.FindByCode(code);

        if (node == null)
        {
            // 该硬件的所有节点信息
            var list = Node.Search(di.UUID, di.MachineGuid, di.Macs);

            // 当前节点信息，取较老者
            list = list.OrderBy(e => e.ID).ToList();

            // 找到节点
            node ??= list.FirstOrDefault();
        }

        var name = "";
        if (name.IsNullOrEmpty()) name = di.MachineName;
        if (name.IsNullOrEmpty()) name = di.UserName;

        node ??= new Node
        {
            Enable = true,

            CreateIP = ip,
            CreateTime = DateTime.Now,
        };

        //// 如果未打开动态注册，则把节点修改为禁用
        //node.Enable = set.AutoRegister;

        if (node.Name.IsNullOrEmpty()) node.Name = name;

        // 优先使用节点散列来生成节点证书，确保节点路由到其它接入网关时保持相同证书代码
        node.Code = code;

        node.Secret = Rand.NextString(16);
        node.UpdateIP = ip;
        node.UpdateTime = DateTime.Now;

        node.Save();
        autoReg = true;

        node.WriteHistory("动态注册", true, inf.ToJson(false, false, false), ip);

        return node;
    }
    #endregion

    #region 心跳
    public PingResponse Ping(Node node, PingInfo inf, String token, String ip, ApiSetting set)
    {
        var rs = new PingResponse
        {
            Time = inf.Time,
            ServerTime = DateTime.UtcNow.ToLong(),
        };

        if (node != null)
        {
            if (!inf.IP.IsNullOrEmpty()) node.IP = inf.IP;
            node.UpdateIP = ip;
            node.FixArea();
            //node.FixNameByRule();
            node.SaveAsync();

            rs.Period = node.Period;

            var olt = GetOnline(node, ip) ?? CreateOnline(node, token, ip);
            olt.Name = node.Name;
            olt.Category = node.Category;
            olt.Version = node.Version;
            olt.CompileTime = node.CompileTime;
            olt.Save(null, inf, token, ip);

            // 令牌有效期检查，10分钟内到期的令牌，颁发新令牌。
            //todo 这里将来由客户端提交刷新令牌，才能颁发新的访问令牌。
            var tm = ValidAndIssueToken(node.Code, token, set);
            if (tm != null)
            {
                rs.Token = tm.AccessToken;

                node.WriteHistory("刷新令牌", true, tm.ToJson(), ip);
            }
        }

        return rs;
    }

    /// <summary></summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public NodeOnline GetOnline(Node node, String ip)
    {
        var sid = $"{node.ID}@{ip}";
        var olt = _cacheProvider.Cache.Get<NodeOnline>($"NodeOnline:{sid}");
        if (olt != null)
        {
            _cacheProvider.Cache.SetExpire($"NodeOnline:{sid}", TimeSpan.FromSeconds(600));
            return olt;
        }

        return NodeOnline.FindBySessionID(sid);
    }

    /// <summary>检查在线</summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public NodeOnline CreateOnline(Node node, String token, String ip)
    {
        var sid = $"{node.ID}@{ip}";
        var olt = NodeOnline.GetOrAdd(sid);
        olt.NodeID = node.ID;
        olt.Name = node.Name;
        olt.IP = node.IP;
        olt.Category = node.Category;
        olt.ProvinceID = node.ProvinceID;
        olt.CityID = node.CityID;

        olt.Version = node.Version;
        olt.CompileTime = node.CompileTime;
        olt.Memory = node.Memory;
        olt.MACs = node.MACs;
        //olt.COMs = node.COMs;
        olt.Token = token;
        olt.CreateIP = ip;

        olt.Creator = Environment.MachineName;

        _cacheProvider.Cache.Set($"NodeOnline:{sid}", olt, 600);

        return olt;
    }
    #endregion

    #region 辅助
    public TokenModel IssueToken(String name, ApiSetting set)
    {
        // 颁发令牌
        var ss = set.TokenSecret.Split(':');
        var jwt = new JwtBuilder
        {
            Issuer = Assembly.GetEntryAssembly().GetName().Name,
            Subject = name,
            Id = Rand.NextString(8),
            Expire = DateTime.Now.AddSeconds(set.TokenExpire),

            Algorithm = ss[0],
            Secret = ss[1],
        };

        return new TokenModel
        {
            AccessToken = jwt.Encode(null),
            TokenType = jwt.Type ?? "JWT",
            ExpireIn = set.TokenExpire,
            RefreshToken = jwt.Encode(null),
        };
    }

    public Node DecodeToken(String token, String tokenSecret)
    {
        if (token.IsNullOrEmpty()) throw new ArgumentNullException(nameof(token));
        //if (token.IsNullOrEmpty()) throw new ApiException(402, $"节点未登录[ip={UserHost}]");

        // 解码令牌
        var ss = tokenSecret.Split(':');
        var jwt = new JwtBuilder
        {
            Algorithm = ss[0],
            Secret = ss[1],
        };

        var rs = jwt.TryDecode(token, out var message);
        var node = Node.FindByCode(jwt.Subject);
        if (!rs || node == null)
            if (node != null)
                throw new ApiException(403, $"[{node.Name}/{node.Code}]非法访问 {message}");
            else
                throw new ApiException(403, $"[{jwt.Subject}]非法访问 {message}");

        return node;
    }

    public TokenModel ValidAndIssueToken(String deviceCode, String token, ApiSetting set)
    {
        if (token.IsNullOrEmpty()) return null;
        //var set = Setting.Current;

        // 令牌有效期检查，10分钟内过期者，重新颁发令牌
        var ss = set.TokenSecret.Split(':');
        var jwt = new JwtBuilder
        {
            Algorithm = ss[0],
            Secret = ss[1],
        };
        var rs = jwt.TryDecode(token, out var message);
        return !rs || jwt == null ? null : DateTime.Now.AddMinutes(10) > jwt.Expire ? IssueToken(deviceCode, set) : null;
    }

    private void WriteHistory(Node node, String action, Boolean success, String remark, String ip)
    {
        var hi = NodeHistory.Create(node, action, success, remark, Environment.MachineName, ip);
        hi.SaveAsync();
    }
    #endregion
}
