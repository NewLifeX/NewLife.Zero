using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife;
using NewLife.Data;
using XCode;
using XCode.Membership;

namespace Zero.Data.Projects
{
    /// <summary>团队成员。每个团队拥有哪些成员，每个成员有一个主力团队</summary>
    public partial class TeamMember : LogEntity<TeamMember>
    {
        #region 对象操作
        static TeamMember()
        {
            // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
            //var df = Meta.Factory.AdditionalFields;
            //df.Add(nameof(TeamId));

            // 过滤器 UserModule、TimeModule、IPModule
            Meta.Modules.Add<UserModule>();
            Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            if (Kind.IsNullOrEmpty()) Kind = Member?.Kind;
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

        /// <summary>成员</summary>
        [XmlIgnore, IgnoreDataMember]
        //[ScriptIgnore]
        public Member Member => Extends.Get(nameof(Member), k => Member.FindByID(MemberId));

        /// <summary>成员</summary>
        [XmlIgnore, IgnoreDataMember]
        //[ScriptIgnore]
        [DisplayName("成员")]
        [Map(nameof(MemberId), typeof(Member), "ID")]
        public String MemberName => Member?.Name;
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static TeamMember FindByID(Int32 id)
        {
            if (id <= 0) return null;

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.ID == id);

            // 单对象缓存
            return Meta.SingleCache[id];

            //return Find(_.ID == id);
        }

        /// <summary>根据团队查找</summary>
        /// <param name="teamId">团队</param>
        /// <returns>实体列表</returns>
        public static IList<TeamMember> FindAllByTeamId(Int32 teamId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.TeamId == teamId);

            return FindAll(_.TeamId == teamId);
        }

        /// <summary>根据成员查找</summary>
        /// <param name="memberId">成员</param>
        /// <returns>实体列表</returns>
        public static IList<TeamMember> FindAllByMemberId(Int32 memberId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.MemberId == memberId);

            return FindAll(_.MemberId == memberId);
        }
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="teamId">团队</param>
        /// <param name="memberId">成员</param>
        /// <param name="enable">启用</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="key">关键字</param>
        /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
        /// <returns>实体列表</returns>
        public static IList<TeamMember> Search(Int32 teamId, Int32 memberId, Boolean? enable, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (teamId >= 0) exp &= _.TeamId == teamId;
            if (memberId >= 0) exp &= _.MemberId == memberId;
            if (enable != null) exp &= _.Enable == enable;
            exp &= _.UpdateTime.Between(start, end);
            if (!key.IsNullOrEmpty()) exp &= _.CreateUser.Contains(key) | _.CreateIP.Contains(key) | _.UpdateUser.Contains(key) | _.UpdateIP.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }

        // Select Count(ID) as ID,Category From TeamMember Where CreateTime>'2020-01-24 00:00:00' Group By Category Order By ID Desc limit 20
        //static readonly FieldCache<TeamMember> _CategoryCache = new FieldCache<TeamMember>(nameof(Category))
        //{
        //Where = _.CreateTime > DateTime.Today.AddDays(-30) & Expression.Empty
        //};

        ///// <summary>获取类别列表，字段缓存10分钟，分组统计数据最多的前20种，用于魔方前台下拉选择</summary>
        ///// <returns></returns>
        //public static IDictionary<String, String> GetCategoryList() => _CategoryCache.FindAllName();
        #endregion

        #region 业务操作
        #endregion
    }
}