using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>产品</summary>
    public partial interface IProduct
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>团队</summary>
        Int32 TeamId { get; set; }

        /// <summary>名称</summary>
        String Name { get; set; }

        /// <summary>类型</summary>
        String Kind { get; set; }

        /// <summary>负责人</summary>
        Int32 LeaderId { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>版本数</summary>
        Int32 Versions { get; set; }

        /// <summary>故事数</summary>
        Int32 Stories { get; set; }

        /// <summary>完成</summary>
        Boolean Completed { get; set; }

        /// <summary>备注</summary>
        String Remark { get; set; }
        #endregion
    }
}