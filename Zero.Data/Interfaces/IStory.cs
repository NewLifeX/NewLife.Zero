using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>故事。用户故事的目标是将特定价值提供给客户，不必是传统意义上的外部最终用户，也可以是依赖您团队的组织内部客户或同事。用户故事是简单语言中的几句话，概述了所需的结果。</summary>
    public partial interface IStory
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>产品</summary>
        Int32 ProductId { get; set; }

        /// <summary>版本</summary>
        Int32 VersionId { get; set; }

        /// <summary>处理人</summary>
        Int32 MemberId { get; set; }

        /// <summary>事项</summary>
        String Title { get; set; }

        /// <summary>开始日期</summary>
        DateTime StartDate { get; set; }

        /// <summary>结束日期</summary>
        DateTime EndDate { get; set; }

        /// <summary>工时</summary>
        Int32 ManHours { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>备注</summary>
        String Remark { get; set; }
        #endregion
    }
}