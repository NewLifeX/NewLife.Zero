using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NewLife;
using NewLife.Data;
using XCode;
using XCode.Cache;
using XCode.Membership;

namespace Zero.Data.Projects
{
    /// <summary>版本计划</summary>
    public partial class VersionPlan : LogEntity<VersionPlan>
    {
        #region 对象操作
        static VersionPlan()
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
        }
        #endregion

        #region 扩展属性
        /// <summary>团队</summary>
        [IgnoreDataMember]
        public Team Team => Extends.Get(nameof(Team), k => Team.FindByID(TeamId));

        /// <summary>团队</summary>
        [IgnoreDataMember]
        [Map(nameof(TeamId), typeof(Team), "ID")]
        public String TeamName => Team?.Name;

        /// <summary>产品</summary>
        [IgnoreDataMember]
        public Product Product => Extends.Get(nameof(Product), k => Product.FindByID(ProductId));

        /// <summary>产品</summary>
        [IgnoreDataMember]
        [Map(nameof(ProductId), typeof(Product), "ID")]
        public String ProductName => Product?.Name;
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static VersionPlan FindByID(Int32 id)
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
        public static IList<VersionPlan> FindAllByTeamId(Int32 teamId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.TeamId == teamId);

            return FindAll(_.TeamId == teamId);
        }

        /// <summary>根据产品查找</summary>
        /// <param name="productId">产品</param>
        /// <returns>实体列表</returns>
        public static IList<VersionPlan> FindAllByProductId(Int32 productId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.ProductId == productId);

            return FindAll(_.ProductId == productId);
        }
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="teamId">团队</param>
        /// <param name="productId">产品</param>
        /// <param name="kind">类型</param>
        /// <param name="enable">启用</param>
        /// <param name="completed">完成</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="key">关键字</param>
        /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
        /// <returns>实体列表</returns>
        public static IList<VersionPlan> Search(Int32 teamId, Int32 productId, String kind, Boolean? enable, Boolean? completed, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (teamId >= 0) exp &= _.TeamId == teamId;
            if (productId >= 0) exp &= _.ProductId == productId;
            if (!kind.IsNullOrEmpty()) exp &= _.Kind == kind;
            if (enable != null) exp &= _.Enable == enable;
            if (completed != null) exp &= _.Completed == completed;
            exp &= _.UpdateTime.Between(start, end);
            if (!key.IsNullOrEmpty()) exp &= _.Name.Contains(key) | _.CreateUser.Contains(key) | _.CreateIP.Contains(key) | _.UpdateUser.Contains(key) | _.UpdateIP.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }

        /// <summary>高级查询</summary>
        /// <param name="teamId">团队</param>
        /// <param name="productId">产品</param>
        /// <param name="kind">类型</param>
        /// <param name="enable">启用</param>
        /// <param name="completed">完成</param>
        /// <returns>实体列表</returns>
        public static IList<VersionPlan> Search(Int32 teamId, Int32 productId, String kind, Boolean? enable, Boolean? completed)
        {
            var exp = new WhereExpression();

            if (teamId >= 0) exp &= _.TeamId == teamId;
            if (productId >= 0) exp &= _.ProductId == productId;
            if (!kind.IsNullOrEmpty()) exp &= _.Kind == kind;
            if (enable != null) exp &= _.Enable == enable;
            if (completed != null) exp &= _.Completed == completed;

            return FindAll(exp, null);
        }

        /// <summary>查找未完成的版本</summary>
        /// <param name="teamId">团队</param>
        /// <param name="productId">产品</param>
        /// <returns>实体列表</returns>
        public static IList<VersionPlan> FindAllNotCompleted(Int32 teamId, Int32 productId) => Search(teamId, productId, null, true, false);

        // Select Count(ID) as ID,Category From VersionPlan Where CreateTime>'2020-01-24 00:00:00' Group By Category Order By ID Desc limit 20
        static readonly FieldCache<VersionPlan> _KindCache = new(nameof(Kind))
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

            // 修正故事数
            var list = Story.FindAllNotCompleted(-1, ID);
            Stories = list.Count;
            ManHours = list.Sum(e => e.ManHours);
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