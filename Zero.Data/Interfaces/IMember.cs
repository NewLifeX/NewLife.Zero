using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>成员。所有可用团队成员</summary>
    public partial interface IMember
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>名称</summary>
        String Name { get; set; }

        /// <summary>类型</summary>
        String Kind { get; set; }

        /// <summary>团队。所属主团队</summary>
        Int32 TeamId { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>团队数。所在团队总数，含协助团队</summary>
        Int32 Teams { get; set; }

        /// <summary>用户。所属登录用户</summary>
        Int32 UserId { get; set; }

        /// <summary>用户名</summary>
        String UserName { get; set; }

        /// <summary>备注</summary>
        String Remark { get; set; }
        #endregion
    }
}