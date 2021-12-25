using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife;
using NewLife.Data;
using XCode;
using XCode.Cache;
using XCode.Membership;

namespace Zero.Data.Projects
{
    /// <summary>成员。所有可用团队成员</summary>
    public partial class Member : LogEntity<Member>
    {
        #region 对象操作
        static Member()
        {
            // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
            //var df = Meta.Factory.AdditionalFields;
            //df.Add(nameof(TeamId));

            // 过滤器 UserModule、TimeModule、IPModule
            Meta.Modules.Add<UserModule>();
            Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();

            // 单对象缓存
            var sc = Meta.SingleCache;
            sc.FindSlaveKeyMethod = k => Find(_.Name == k);
            sc.GetSlaveKeyMethod = e => e.Name;
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            // 这里验证参数范围，建议抛出参数异常，指定参数名，前端用户界面可以捕获参数异常并聚焦到对应的参数输入框
            if (Name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(Name), "名称不能为空！");

            // 在新插入数据或者修改了指定字段时进行修正
            // 处理当前已登录用户信息，可以由UserModule过滤器代劳
            /*var user = ManageProvider.User;
            if (user != null)
            {
                if (isNew && !Dirtys[nameof(CreateUserID)]) CreateUserID = user.ID;
                if (!Dirtys[nameof(UpdateUserID)]) UpdateUserID = user.ID;
            }*/
            //if (isNew && !Dirtys[nameof(CreateTime)]) CreateTime = DateTime.Now;
            //if (!Dirtys[nameof(UpdateTime)]) UpdateTime = DateTime.Now;
            //if (isNew && !Dirtys[nameof(CreateIP)]) CreateIP = ManageProvider.UserHost;
            //if (!Dirtys[nameof(UpdateIP)]) UpdateIP = ManageProvider.UserHost;

            // 检查唯一索引
            // CheckExist(isNew, nameof(Name));
        }
        #endregion

        #region 扩展属性
        /// <summary>团队</summary>
        [XmlIgnore, IgnoreDataMember]
        //[ScriptIgnore]
        public Team Team => Extends.Get(nameof(Team), k => Team.FindByID(TeamId));

        /// <summary>团队</summary>
        [XmlIgnore, IgnoreDataMember]
        //[ScriptIgnore]
        [DisplayName("团队")]
        [Map(nameof(TeamId), typeof(Team), "ID")]
        public String TeamName => Team?.Name;
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static Member FindByID(Int32 id)
        {
            if (id <= 0) return null;

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.ID == id);

            // 单对象缓存
            return Meta.SingleCache[id];

            //return Find(_.ID == id);
        }

        /// <summary>根据名称查找</summary>
        /// <param name="name">名称</param>
        /// <returns>实体对象</returns>
        public static Member FindByName(String name)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Name == name);

            // 单对象缓存
            //return Meta.SingleCache.GetItemWithSlaveKey(name) as Member;

            return Find(_.Name == name);
        }

        /// <summary>查找用户绑定的成员</summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Member FindByUserId(Int32 userId)
        {
            if (userId == 0) return null;

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.UserId == userId);

            // 单对象缓存
            //return Meta.SingleCache.GetItemWithSlaveKey(name) as Member;

            return Find(_.UserId == userId);
        }
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="teamId"></param>
        /// <param name="kind"></param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IList<Member> Search(Int32 teamId, String kind, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (teamId >= 0) exp &= _.TeamId == teamId;
            if (!kind.IsNullOrEmpty()) exp &= _.Kind == kind;
            exp &= _.UpdateTime.Between(start, end);
            if (!key.IsNullOrEmpty()) exp &= _.Kind.Contains(key) | _.Name.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }

        static readonly FieldCache<Member> _KindCache = new(nameof(Kind))
        {
            Where = _.CreateTime > DateTime.Today.AddDays(-30) & Expression.Empty
        };

        /// <summary>获取类别列表，字段缓存10分钟，分组统计数据最多的前20种，用于魔方前台下拉选择</summary>
        /// <returns></returns>
        public static IDictionary<String, String> GetKinds() => _KindCache.FindAllName();
        #endregion

        #region 业务操作
        /// <summary>刷新数据</summary>
        public void Refresh()
        {
            if (ID == 0) return;

            var list = TeamMember.FindAllByMemberId(ID);

            // 修正
            Teams = list.Count(e => e.Enable);
        }

        /// <summary>修正数据，刷新并保存</summary>
        /// <returns></returns>
        public Int32 Fix()
        {
            Refresh();

            return Update();
        }
        #endregion
    }
}