using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NewLife;
using NewLife.Data;
using XCode;
using XCode.Membership;

namespace Zero.Data.Projects
{
    /// <summary>故事。用户故事的目标是将特定价值提供给客户，不必是传统意义上的外部最终用户，也可以是依赖您团队的组织内部客户或同事。用户故事是简单语言中的几句话，概述了所需的结果。</summary>
    public partial class Story : LogEntity<Story>
    {
        #region 对象操作
        static Story()
        {
            // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
            //var df = Meta.Factory.AdditionalFields;
            //df.Add(nameof(VersionId));

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

            var ver = Version;
            if (ver != null)
            {
                ProductId = ver.ProductId;
            }
        }
        #endregion

        #region 扩展属性
        /// <summary>版本</summary>
        [IgnoreDataMember]
        public VersionPlan Version => Extends.Get(nameof(Version), k => VersionPlan.FindByID(VersionId));

        /// <summary>版本</summary>
        [IgnoreDataMember]
        [Map(nameof(VersionId))]
        public String VersionName => Version?.Name;

        /// <summary>成员</summary>
        [IgnoreDataMember]
        public Member Member => Extends.Get(nameof(Member), k => Member.FindByID(MemberId));

        /// <summary>成员</summary>
        [IgnoreDataMember]
        [Map(nameof(MemberId), typeof(Member), "ID")]
        public String MemberName => Member?.Name;

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
        public static Story FindByID(Int32 id)
        {
            if (id <= 0) return null;

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.ID == id);

            // 单对象缓存
            return Meta.SingleCache[id];

            //return Find(_.ID == id);
        }

        /// <summary>根据产品查找</summary>
        /// <param name="productId">成员</param>
        /// <returns>实体列表</returns>
        public static IList<Story> FindAllByProductId(Int32 productId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.ProductId == productId);

            return FindAll(_.ProductId == productId);
        }

        /// <summary>根据版本查找</summary>
        /// <param name="versionId">版本</param>
        /// <returns>实体列表</returns>
        public static IList<Story> FindAllByVersionId(Int32 versionId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.VersionId == versionId);

            return FindAll(_.VersionId == versionId);
        }

        /// <summary>根据成员查找</summary>
        /// <param name="memberId">成员</param>
        /// <returns>实体列表</returns>
        public static IList<Story> FindAllByMemberId(Int32 memberId)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.MemberId == memberId);

            return FindAll(_.MemberId == memberId);
        }
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="productId">产品</param>
        /// <param name="versionId">版本</param>
        /// <param name="memberId">成员</param>
        /// <param name="enable">启用</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="key">关键字</param>
        /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
        /// <returns>实体列表</returns>
        public static IList<Story> Search(Int32 productId, Int32 versionId, Int32 memberId, Boolean? enable, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (productId >= 0) exp &= _.ProductId == productId;
            if (versionId >= 0) exp &= _.VersionId == versionId;
            if (memberId >= 0) exp &= _.MemberId == memberId;
            if (enable != null) exp &= _.Enable == enable;
            exp &= _.UpdateTime.Between(start, end);
            if (!key.IsNullOrEmpty()) exp &= _.CreateUser.Contains(key) | _.CreateIP.Contains(key) | _.UpdateUser.Contains(key) | _.UpdateIP.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }

        /// <summary>查找未完成的故事</summary>
        /// <param name="productId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static IList<Story> FindAllNotCompleted(Int32 productId, Int32 versionId) => Search(productId, versionId, -1, true, DateTime.MinValue, DateTime.MinValue, null, null);

        /// <summary>查找指定版本的故事</summary>
        /// <param name="versions"></param>
        /// <returns></returns>
        public static IList<Story> Search(Int32[] versions)
        {
            if (versions == null || versions.Length == 0) return new List<Story>();

            return FindAll(_.VersionId.In(versions));
        }
        // Select Count(ID) as ID,Category From VersionMember Where CreateTime>'2020-01-24 00:00:00' Group By Category Order By ID Desc limit 20
        //static readonly FieldCache<VersionMember> _CategoryCache = new FieldCache<VersionMember>(nameof(Category))
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