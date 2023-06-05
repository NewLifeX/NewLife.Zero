using NewLife;
using NewLife.Data;
using Stardust.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using XCode;
using XCode.Cache;
using XCode.Membership;

namespace Zero.Data.Nodes
{
    /// <summary>节点信息</summary>
    public partial class Node : Entity<Node>
    {
        #region 对象操作
        static Node()
        {
            var df = Meta.Factory.AdditionalFields;
            df.Add(__.Logins);
            //!!! OnlineTime是新加字段，允许空，导致累加操作失败，暂时关闭累加
            //df.Add(__.OnlineTime);

            Meta.Modules.Add<UserModule>();
            Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();

            var sc = Meta.SingleCache;
            sc.Expire = 30 * 60;
            sc.FindSlaveKeyMethod = e => Find(__.Code, e);
            sc.GetSlaveKeyMethod = e => e.Code;
            //sc.SlaveKeyIgnoreCase = false;
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew"></param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            if (Name.IsNullOrEmpty()) throw new ArgumentNullException(__.Name, _.Name.DisplayName + "不能为空！");

            //var len = _.CpuID.Length;
            //if (CpuID != null && len > 0 && CpuID.Length > len) CpuID = CpuID.Substring(0, len);

            var len = _.Uuid.Length;
            if (Uuid != null && len > 0 && Uuid.Length > len) Uuid = Uuid.Substring(0, len);

            len = _.MachineGuid.Length;
            if (MachineGuid != null && len > 0 && MachineGuid.Length > len) MachineGuid = MachineGuid.Substring(0, len);

            len = _.MACs.Length;
            if (MACs != null && len > 0 && MACs.Length > len) MACs = MACs.Substring(0, len);

            len = _.DiskID.Length;
            if (DiskID != null && len > 0 && DiskID.Length > len) DiskID = DiskID.Substring(0, len);

            len = _.OS.Length;
            if (OS != null && len > 0 && OS.Length > len) OS = OS.Substring(0, len);

            // 建议先调用基类方法，基类方法会做一些统一处理
            base.Valid(isNew);

            if (Period == 0) Period = 60;
        }

        /// <summary>已重载</summary>
        /// <returns></returns>
        public override String ToString() => Code;
        #endregion

        #region 扩展属性
        /// <summary>省份</summary>
        [XmlIgnore, ScriptIgnore]
        public Area Province => Extends.Get(nameof(Province), k => Area.FindByID(ProvinceID));

        /// <summary>省份名</summary>
        [Map(__.ProvinceID)]
        public String ProvinceName => Province + "";

        /// <summary>城市</summary>
        [XmlIgnore, ScriptIgnore]
        public Area City => Extends.Get(nameof(City), k => Area.FindByID(CityID));

        /// <summary>城市名</summary>
        [Map(__.CityID)]
        public String CityName => City?.Path;

        /// <summary>最后地址。IP=>Address</summary>
        [DisplayName("最后地址")]
        public String LastLoginAddress => LastLoginIP.IPToAddress();
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public static Node FindByID(Int32 id)
        {
            if (id <= 0) return null;

            if (Meta.Count < 1000) return Meta.Cache.Entities.FirstOrDefault(e => e.ID == id);

            // 单对象缓存
            return Meta.SingleCache[id];
        }

        /// <summary>根据名称。登录用户名查找</summary>
        /// <param name="name">名称。登录用户名</param>
        /// <returns></returns>
        public static Node FindByName(String name)
        {
            if (name.IsNullOrEmpty()) return null;

            if (Meta.Count < 1000) return Meta.Cache.Entities.FirstOrDefault(e => e.Name == name);

            return Find(__.Name, name);
        }

        /// <summary>根据Mac</summary>
        /// <param name="mac">Mac</param>
        /// <returns></returns>
        public static Node FindByMac(String mac)
        {
            if (mac.IsNullOrEmpty()) return null;

            return Find(_.MACs.Contains(mac));
        }

        /// <summary>根据名称查找</summary>
        /// <param name="code">名称</param>
        /// <param name="cache">是否走缓存</param>
        /// <returns>实体对象</returns>
        public static Node FindByCode(String code, Boolean cache = true)
        {
            if (code.IsNullOrEmpty()) return null;

            if (!cache) return Find(_.Code == code);

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Code == code);

            // 单对象缓存
            return Meta.SingleCache.GetItemWithSlaveKey(code) as Node;
        }

        /// <summary>根据IP查找节点</summary>
        /// <param name="ips"></param>
        /// <returns></returns>
        public static IList<Node> FindAllByIPs(params String[] ips)
        {
            if (ips == null || ips.Length == 0) return new List<Node>();

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => !e.IP.IsNullOrEmpty() && ips.Contains(e.IP));

            return FindAll(_.IP.In(ips));
        }

        /// <summary>根据IP查找节点</summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static IList<Node> FindAllByIP(String ip)
        {
            if (ip.IsNullOrEmpty()) return new List<Node>();

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => ip == e.IP);

            return FindAll(_.IP == ip);
        }

        /// <summary>根据分类查找</summary>
        /// <param name="category">分类</param>
        /// <returns>实体列表</returns>
        public static IList<Node> FindAllByCategory(String category)
        {
            if (category.IsNullOrEmpty()) return new List<Node>();

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.Category.EqualIgnoreCase(category));

            return FindAll(_.Category == category);
        }

        /// <summary>根据产品查找</summary>
        /// <param name="productCode">产品</param>
        /// <returns>实体列表</returns>
        public static IList<Node> FindAllByProductCode(String productCode)
        {
            if (productCode.IsNullOrEmpty()) return new List<Node>();

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.ProductCode.EqualIgnoreCase(productCode));

            return FindAll(_.ProductCode == productCode);
        }

    /// <summary>根据唯一标识、机器标识、网卡查找</summary>
    /// <param name="uuid">唯一标识</param>
    /// <param name="machineGuid">机器标识</param>
    /// <param name="mACs">网卡</param>
    /// <returns>实体列表</returns>
    public static IList<Node> FindAllByUuidAndMachineGuidAndMACs(String uuid, String machineGuid, String mACs)
    {
        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.Uuid.EqualIgnoreCase(uuid) && e.MachineGuid.EqualIgnoreCase(machineGuid) && e.MACs.EqualIgnoreCase(mACs));

        return FindAll(_.Uuid == uuid & _.MachineGuid == machineGuid & _.MACs == mACs);
    }
        #endregion

        #region 高级查询
        /// <summary>根据唯一标识搜索，任意一个匹配即可</summary>
        /// <param name="uuid"></param>
        /// <param name="guid"></param>
        /// <param name="macs"></param>
        /// <returns></returns>
        public static IList<Node> Search(String uuid, String guid, String macs)
        {
            var exp = new WhereExpression();
            if (!uuid.IsNullOrEmpty()) exp &= _.Uuid == uuid;
            if (!guid.IsNullOrEmpty()) exp &= _.MachineGuid == guid;
            if (!macs.IsNullOrEmpty()) exp &= _.MACs == macs;

            if (exp.IsEmpty) return new List<Node>();

            return FindAll(exp);
        }

        /// <summary>高级查询</summary>
        /// <param name="provinceId">省份</param>
        /// <param name="cityId">城市</param>
        /// <param name="category">类别</param>
        /// <param name="product">类别</param>
        /// <param name="version">版本</param>
        /// <param name="enable"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IList<Node> Search(Int32 provinceId, Int32 cityId, String category, String product, String version, Boolean? enable, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (provinceId >= 0) exp &= _.ProvinceID == provinceId;
            if (cityId >= 0) exp &= _.CityID == cityId;
            if (!category.IsNullOrEmpty()) exp &= _.Category == category;
            if (!product.IsNullOrEmpty()) exp &= _.ProductCode == product;
            if (!version.IsNullOrEmpty()) exp &= _.Version == version;
            if (enable != null) exp &= _.Enable == enable.Value;

            //exp &= _.CreateTime.Between(start, end);
            exp &= _.LastLogin.Between(start, end);

            if (!key.IsNullOrEmpty()) exp &= SearchWhereByKeys(key);

            return FindAll(exp, page);
        }

        /// <summary>根据IP查找节点</summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static IList<Node> SearchByIP(String ip)
        {
            if (ip.IsNullOrEmpty()) return new List<Node>();

            var ips = ip.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // 模糊匹配IP
            IList<Node> list;
            if (Meta.Session.Count < 1000)
            {
                list = Meta.Cache.FindAll(e => !e.IP.IsNullOrEmpty() && ips.Any(y => e.IP.Contains(y)));
            }
            else
            {
                var exp = new WhereExpression();
                foreach (var item in ips)
                {
                    exp |= _.IP.Contains(item);
                }
                list = FindAll(exp);
            }

            // 精确匹配IP
            list = list.Where(e => !e.IP.IsNullOrEmpty() && e.IP.Split(',').Any(y => ips.Contains(y))).ToList();

            return list;
        }

        /// <summary>根据类别搜索</summary>
        /// <param name="category"></param>
        /// <param name="enable"></param>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IList<Node> SearchByCategory(String category, Boolean? enable, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (!category.IsNullOrEmpty()) exp &= _.Category == category | _.Category.IsNullOrEmpty();

            if (enable != null) exp &= _.Enable == enable.Value;

            if (!key.IsNullOrEmpty()) exp &= SearchWhereByKeys(key);

            return FindAll(exp, page);
        }

        internal static IList<Node> SearchByCreateDate(DateTime date)
        {
            // 先用带有索引的UpdateTime过滤一次
            return FindAll(_.UpdateTime >= date & _.CreateTime.Between(date, date));
        }

        internal static IDictionary<Int32, Int32> SearchGroupByCreateTime(DateTime start, DateTime end)
        {
            var exp = new WhereExpression();
            exp &= _.CreateTime.Between(start, end);
            var list = FindAll(exp.GroupBy(_.ProvinceID), null, _.ID.Count() & _.ProvinceID, 0, 0);
            return list.ToDictionary(e => e.ProvinceID, e => e.ID);
        }

        internal static IDictionary<Int32, Int32> SearchGroupByLastLogin(DateTime start, DateTime end)
        {
            var exp = new WhereExpression();
            exp &= _.LastLogin.Between(start, end);
            var list = FindAll(exp.GroupBy(_.ProvinceID), null, _.ID.Count() & _.ProvinceID, 0, 0);
            return list.ToDictionary(e => e.ProvinceID, e => e.ID);
        }

        internal static IDictionary<Int32, Int32> SearchCountByCreateDate(DateTime date)
        {
            var exp = new WhereExpression();
            exp &= _.CreateTime < date.AddDays(1);
            var list = FindAll(exp.GroupBy(_.ProvinceID), null, _.ID.Count() & _.ProvinceID, 0, 0);
            return list.ToDictionary(e => e.ProvinceID, e => e.ID);
        }
        #endregion

        #region 扩展操作
        /// <summary>类别名实体缓存，异步，缓存10分钟</summary>
        static Lazy<FieldCache<Node>> VersionCache = new(() => new FieldCache<Node>(__.Version)
        {
            Where = _.UpdateTime > DateTime.Today.AddDays(-30) & Expression.Empty,
            MaxRows = 50
        });

        /// <summary>获取所有类别名称</summary>
        /// <returns></returns>
        public static IDictionary<String, String> FindAllVersion() => VersionCache.Value.FindAllName().OrderByDescending(e => e.Key).ToDictionary(e => e.Key, e => e.Value);

        /// <summary>类别名实体缓存，异步，缓存10分钟</summary>
        static Lazy<FieldCache<Node>> CategoryCache = new(() => new FieldCache<Node>(__.Category)
        {
            Where = _.UpdateTime > DateTime.Today.AddDays(-30) & Expression.Empty,
            MaxRows = 50
        });

        /// <summary>获取所有类别名称</summary>
        /// <returns></returns>
        public static IDictionary<String, String> FindAllCategory() => CategoryCache.Value.FindAllName();

        /// <summary>类别名实体缓存，异步，缓存10分钟</summary>
        static Lazy<FieldCache<Node>> ProductCache = new(() => new FieldCache<Node>(__.ProductCode)
        {
            Where = _.UpdateTime > DateTime.Today.AddDays(-30) & Expression.Empty,
            MaxRows = 50
        });

        /// <summary>获取所有类别名称</summary>
        /// <returns></returns>
        public static IDictionary<String, String> FindAllProduct() => ProductCache.Value.FindAllName();
        #endregion

        #region 业务
        /// <summary>根据编码查询或添加</summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Node GetOrAdd(String code) => GetOrAdd(code, FindByCode, k => new Node { Code = k, Enable = true });

        /// <summary>登录并保存信息</summary>
        /// <param name="di"></param>
        /// <param name="ip"></param>
        public void Login(NodeInfo di, String ip)
        {
            var node = this;

            node.Fill(di);

            // 如果节点本地IP为空，而来源IP是局域网，则直接取用
            //if (node.IP.IsNullOrEmpty() && ip.StartsWithIgnoreCase("10.", "192.", "172.")) node.IP = ip;
            if (node.IP.IsNullOrEmpty()) node.IP = ip;

            node.Logins++;
            node.LastLogin = DateTime.Now;
            node.LastLoginIP = ip;

            if (node.CreateIP.IsNullOrEmpty()) node.CreateIP = ip;
            node.UpdateIP = ip;

            node.FixArea();

            node.Save();
        }

        /// <summary>填充</summary>
        /// <param name="di"></param>
        public void Fill(NodeInfo di)
        {
            var node = this;

            if (!di.OSName.IsNullOrEmpty()) node.OS = di.OSName;
            if (!di.OSVersion.IsNullOrEmpty()) node.OSVersion = di.OSVersion;
            if (!di.Architecture.IsNullOrEmpty()) node.Architecture = di.Architecture;
            if (!di.Version.IsNullOrEmpty()) node.Version = di.Version;
            if (di.Compile.Year > 2000) node.CompileTime = di.Compile;

            if (!di.MachineName.IsNullOrEmpty()) node.MachineName = di.MachineName;
            if (!di.UserName.IsNullOrEmpty()) node.UserName = di.UserName;
            if (!di.IP.IsNullOrEmpty()) node.IP = di.IP;
            if (!di.Processor.IsNullOrEmpty()) node.Processor = di.Processor;
            //if (!di.CpuID.IsNullOrEmpty()) node.CpuID = di.CpuID;
            if (!di.UUID.IsNullOrEmpty()) node.Uuid = di.UUID;
            if (!di.MachineGuid.IsNullOrEmpty()) node.MachineGuid = di.MachineGuid;
            if (!di.DiskID.IsNullOrEmpty()) node.DiskID = di.DiskID;

            if (di.ProcessorCount > 0) node.Cpu = di.ProcessorCount;
            if (di.Memory > 0) node.Memory = (Int32)(di.Memory / 1024 / 1024);
            if (di.TotalSize > 0) node.TotalSize = (Int32)(di.TotalSize / 1024 / 1024);
            if (di.MaxOpenFiles > 0) node.MaxOpenFiles = di.MaxOpenFiles;
            if (!di.Dpi.IsNullOrEmpty()) node.Dpi = di.Dpi;
            if (!di.Resolution.IsNullOrEmpty()) node.Resolution = di.Resolution;
            if (!di.Macs.IsNullOrEmpty()) node.MACs = di.Macs;
            //if (!di.COMs.IsNullOrEmpty()) node.COMs = di.COMs;
            if (!di.InstallPath.IsNullOrEmpty()) node.InstallPath = di.InstallPath;
            if (!di.Runtime.IsNullOrEmpty()) node.Runtime = di.Runtime;
            if (!di.Framework.IsNullOrEmpty()) node.Framework = di.Framework;
        }

        /// <summary>修正地区</summary>
        public void FixArea()
        {
            var node = this;
            if (node.UpdateIP.IsNullOrEmpty()) return;

            var rs = Area.SearchIP(node.UpdateIP);
            if (rs.Count > 0) node.ProvinceID = rs[0].ID;
            if (rs.Count > 1) node.CityID = rs[^1].ID;
        }

        ///// <summary>
        ///// 根据IP地址修正名称和分类
        ///// </summary>
        //public void FixNameByRule()
        //{
        //    //var ip = IP;
        //    //if (ip.IsNullOrEmpty()) return;

        //    var rule = NodeResolver.Instance.Match(IP, UpdateIP);
        //    if (rule != null)
        //    {
        //        if ((Name.IsNullOrEmpty() || Name == MachineName) && !rule.Name.IsNullOrEmpty())
        //            Name = rule.Name;

        //        if (!rule.Category.IsNullOrEmpty())
        //            Category = rule.Category;
        //    }
        //}

        /// <summary>写历史</summary>
        /// <param name="action"></param>
        /// <param name="success"></param>
        /// <param name="remark"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public NodeHistory WriteHistory(String action, Boolean success, String remark, String ip)
        {
            var hi = NodeHistory.Create(this, action, success, remark, Environment.MachineName, ip);
            hi.SaveAsync();

            return hi;
        }
        #endregion
    }
}