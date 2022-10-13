using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>版本计划</summary>
    public partial interface IVersionPlan
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>团队</summary>
        Int32 TeamId { get; set; }

        /// <summary>产品</summary>
        Int32 ProductId { get; set; }

        /// <summary>名称。版本号</summary>
        String Name { get; set; }

        /// <summary>类型</summary>
        String Kind { get; set; }

        /// <summary>开始日期</summary>
        DateTime StartDate { get; set; }

        /// <summary>结束日期</summary>
        DateTime EndDate { get; set; }

        /// <summary>工时</summary>
        Int32 ManHours { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>完成</summary>
        Boolean Completed { get; set; }

        /// <summary>故事数</summary>
        Int32 Stories { get; set; }

        /// <summary>备注</summary>
        String Remark { get; set; }
        #endregion
    }
}