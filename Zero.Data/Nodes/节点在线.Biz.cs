using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife;
using NewLife.Data;
using NewLife.Remoting.Models;
using NewLife.Serialization;
using XCode;
using XCode.Membership;
using Zero.Data.Models;

namespace Zero.Data.Nodes;

/// <summary>节点在线</summary>
public partial class NodeOnline : Entity<NodeOnline>, IOnlineModel2
{
    #region 对象操作
    static NodeOnline()
    {
        var df = Meta.Factory.AdditionalFields;
        df.Add(__.PingCount);

        Meta.Interceptors.Add<TimeInterceptor>();
        Meta.Interceptors.Add<IPInterceptor>();

        var sc = Meta.SingleCache;
        sc.FindSlaveKeyMethod = k => Find(_.SessionId == k);
        sc.GetSlaveKeyMethod = e => e.SessionId;
    }

    /// <summary>校验数据</summary>
    /// <param name="isNew"></param>
    public override void Valid(Boolean isNew)
    {
        // 截取部分进程字段，避免过长无法保存
        var len = _.Processes.Length;
        if (len > 0 && Processes != null && Processes.Length > len) Processes = Processes[..len];

        len = _.MACs.Length;
        if (len > 0 && MACs != null && MACs.Length > len) MACs = MACs[..len];
        //if (COMs != null && COMs.Length > 200) COMs = COMs.Substring(0, 199);

        base.Valid(isNew);
    }
    #endregion

    #region 扩展属性
    /// <summary>节点</summary>
    [XmlIgnore, ScriptIgnore]
    public Node Node => Extends.Get(nameof(Node), k => Node.FindByID(NodeId));

    /// <summary>节点</summary>
    [Map(__.NodeId)]
    public String NodeName => Node + "";

    /// <summary>省份</summary>
    [XmlIgnore, IgnoreDataMember]
    public Area Province => Extends.Get(nameof(Province), k => Area.FindByID(ProvinceID));

    /// <summary>省份名</summary>
    [Map(__.ProvinceID)]
    public String ProvinceName => Province + "";

    /// <summary>城市</summary>
    [XmlIgnore, IgnoreDataMember]
    public Area City => Extends.Get(nameof(City), k => Area.FindByID(CityID));

    /// <summary>城市名</summary>
    [Map(__.CityID)]
    public String CityName => City?.Path;
    #endregion

    #region 扩展查询
    /// <summary>根据会话查找</summary>
    /// <param name="deviceid">会话</param>
    /// <returns></returns>
    public static NodeOnline FindByNodeId(Int32 deviceid) => Find(__.NodeId, deviceid);

    /// <summary>根据会话查找</summary>
    /// <param name="sessionId">会话</param>
    /// <param name="cache">是否走缓存</param>
    /// <returns></returns>
    public static NodeOnline FindBySessionIdWithCache(String sessionId, Boolean cache = true)
    {
        if (sessionId.IsNullOrEmpty()) return null;

        if (!cache) return Find(_.SessionId == sessionId);

        return Meta.SingleCache.GetItemWithSlaveKey(sessionId) as NodeOnline;
    }

    /// <summary>根据会话查找</summary>
    /// <param name="sessionid">会话</param>
    /// <param name="cache">是否走缓存</param>
    /// <returns></returns>
    public static NodeOnline FindBySessionId(String sessionid, Boolean cache = true)
    {
        if (!cache) return Find(_.SessionId == sessionid);

        return Meta.SingleCache.GetItemWithSlaveKey(sessionid) as NodeOnline;
    }

    /// <summary>根据节点查找所有在线记录</summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public static IList<NodeOnline> FindAllByNodeId(Int32 nodeId) => FindAll(_.NodeId == nodeId);

    /// <summary>根据编号查找</summary>
    /// <param name="id">编号</param>
    /// <returns>实体对象</returns>
    public static NodeOnline FindById(Int32 id)
    {
        if (id <= 0) return null;

        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Id == id);

        // 单对象缓存
        return Meta.SingleCache[id];

        //return Find(_.ID == id);
    }

    /// <summary>根据令牌查找</summary>
    /// <param name="token">令牌</param>
    /// <returns>实体列表</returns>
    public static IList<NodeOnline> FindAllByToken(String token)
    {
        if (token.IsNullOrEmpty()) return [];

        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.Token.EqualIgnoreCase(token));

        return FindAll(_.Token == token);
    }

    /// <summary>根据省份、城市查找</summary>
    /// <param name="provinceId">省份</param>
    /// <param name="cityId">城市</param>
    /// <returns>实体列表</returns>
    public static IList<NodeOnline> FindAllByProvinceIDAndCityID(Int32 provinceId, Int32 cityId)
    {

        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.ProvinceID == provinceId && e.CityID == cityId);

        return FindAll(_.ProvinceID == provinceId & _.CityID == cityId);
    }
    #endregion

    #region 高级查询
    /// <summary>查询满足条件的记录集，分页、排序</summary>
    /// <param name="nodeId">节点</param>
    /// <param name="provinceId">省份</param>
    /// <param name="cityId">城市</param>
    /// <param name="category">类别</param>
    /// <param name="start">开始时间</param>
    /// <param name="end">结束时间</param>
    /// <param name="key">关键字</param>
    /// <param name="page">分页排序参数，同时返回满足条件的总记录数</param>
    /// <returns>实体集</returns>
    public static IList<NodeOnline> Search(Int32 nodeId, Int32 provinceId, Int32 cityId, String category, DateTime start, DateTime end, String key, PageParameter page)
    {
        var exp = new WhereExpression();

        if (nodeId >= 0) exp &= _.NodeId == nodeId;
        if (provinceId >= 0) exp &= _.ProvinceID == provinceId;
        if (cityId >= 0) exp &= _.CityID == cityId;
        if (!category.IsNullOrEmpty()) exp &= _.Category == category;

        exp &= _.CreateTime.Between(start, end);

        if (!key.IsNullOrEmpty()) exp &= _.Name.Contains(key) | _.Data.Contains(key) | _.SessionId.Contains(key);

        return FindAll(exp, page);
    }

    /// <summary>根据产品，分组统计在线数</summary>
    /// <returns></returns>
    public static IDictionary<Int32, Int32> SearchGroupByProvince()
    {
        var list = FindAll(_.ProvinceID.GroupBy(), null, _.Id.Count() & _.ProvinceID);
        return list.ToDictionary(e => e.ProvinceID, e => e.Id);
    }
    #endregion

    #region 业务操作
    /// <summary>根据编码查询或添加</summary>
    /// <param name="sessionid"></param>
    /// <returns></returns>
    public static NodeOnline GetOrAdd(String sessionid) => GetOrAdd(sessionid, (k, c) => FindBySessionId(k, c), k => new NodeOnline { SessionId = k });

    /// <summary>删除过期，指定过期时间</summary>
    /// <param name="expire">超时时间，秒</param>
    /// <returns></returns>
    public static IList<NodeOnline> ClearExpire(TimeSpan expire)
    {
        if (Meta.Count == 0) return null;

        // 10分钟不活跃将会被删除
        var exp = _.UpdateTime < DateTime.Now.Subtract(expire);
        var list = FindAll(exp, null, null, 0, 0);
        list.Delete();

        return list;
    }

    /// <summary>更新并保存在线状态</summary>
    /// <param name="login"></param>
    /// <param name="ping"></param>
    /// <param name="token"></param>
    /// <param name="ip"></param>
    public void Save(LoginInfo login, PingInfo ping, String token, String ip)
    {
        var online = this;

        if (login != null)
        {
            online.Fill(login);
            online.LocalTime = login.Time.ToDateTime().ToLocalTime();
            online.MACs = login.Macs;
        }
        else
        {
            online.Fill(ping);
        }

        online.Token = token;
        online.PingCount++;
        online.UpdateIP = ip;

        // 5秒内直接保存
        if (online.CreateTime.AddSeconds(5) > DateTime.Now)
            online.Save();
        else
            online.SaveAsync();
    }

    /// <summary>填充节点信息</summary>
    /// <param name="di"></param>
    public void Fill(LoginInfo di)
    {
        var online = this;

        online.LocalTime = di.Time.ToDateTime().ToLocalTime();
        online.MACs = di.Macs;
        //online.COMs = di.COMs;
        online.IP = di.IP;
    }

    /// <summary>填充在线节点信息</summary>
    /// <param name="inf"></param>
    private void Fill(PingInfo inf)
    {
        var online = this;

        if (inf.AvailableMemory > 0) online.AvailableMemory = (Int32)(inf.AvailableMemory / 1024 / 1024);
        if (inf.AvailableFreeSpace > 0) online.AvailableFreeSpace = (Int32)(inf.AvailableFreeSpace / 1024 / 1024);
        if (inf.CpuRate > 0) online.CpuRate = inf.CpuRate;
        if (inf.Temperature > 0) online.Temperature = inf.Temperature;
        if (inf.Battery > 0) online.Battery = inf.Battery;
        if (inf.UplinkSpeed > 0) online.UplinkSpeed = (Int64)inf.UplinkSpeed;
        if (inf.DownlinkSpeed > 0) online.DownlinkSpeed = (Int64)inf.DownlinkSpeed;
        if (inf.Uptime > 0) online.Uptime = inf.Uptime;
        if (inf.Delay > 0) online.Delay = inf.Delay;

        var dt = inf.Time.ToDateTime().ToLocalTime();
        if (dt.Year > 2000)
        {
            online.LocalTime = dt;
            online.Offset = (Int32)(inf.Time - DateTime.UtcNow.ToLong());
        }

        if (!inf.IP.IsNullOrEmpty()) online.IP = inf.IP;

        var dic = inf.ToDictionary();
        dic.Remove("Processes");
        online.Data = dic.ToJson();
    }

    //private void CreateData(PingInfo inf, String ip)
    //{
    //    var olt = this;

    //    var dt = inf.Time.ToDateTime().ToLocalTime();

    //    // 插入节点数据
    //    var data = new NodeData
    //    {
    //        NodeId = olt.NodeId,
    //        Name = olt.Name,
    //        AvailableMemory = olt.AvailableMemory,
    //        AvailableFreeSpace = olt.AvailableFreeSpace,
    //        CpuRate = inf.CpuRate,
    //        Temperature = inf.Temperature,
    //        Battery = inf.Battery,
    //        UplinkSpeed = (Int64)inf.UplinkSpeed,
    //        DownlinkSpeed = (Int64)inf.DownlinkSpeed,
    //        ProcessCount = inf.ProcessCount,
    //        TcpConnections = inf.TcpConnections,
    //        TcpTimeWait = inf.TcpTimeWait,
    //        TcpCloseWait = inf.TcpCloseWait,
    //        Uptime = inf.Uptime,
    //        Delay = inf.Delay,
    //        LocalTime = dt,
    //        Offset = olt.Offset,
    //        CreateIP = ip,
    //        Creator = Environment.MachineName,
    //    };

    //    data.SaveAsync();
    //}

    public Int32 Save(IPingRequest request, Object context)
    {
        if (context is DeviceContext ctx)
        {
            Token = ctx.Token;
            UpdateIP = ctx.UserHost;

            if (ctx.Device is Node node)
            {
                Name = node.Name;
                Category = node.Category;
                Version = node.Version;
                CompileTime = node.CompileTime;
                OSKind = node.OSKind;
                //Save(null, inf, context.Token, ip);
            }
            if (request is PingInfo ping) Fill(ping);
        }

        PingCount++;

        //SaveAsync();
        return Update();
    }
    #endregion
}