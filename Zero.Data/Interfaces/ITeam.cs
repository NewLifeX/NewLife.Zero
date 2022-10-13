using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>团队。管理一系列相关的产品和应用系统</summary>
    public partial interface ITeam
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>名称</summary>
        String Name { get; set; }

        /// <summary>编码</summary>
        String Code { get; set; }

        /// <summary>组长</summary>
        Int32 LeaderId { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>产品数</summary>
        Int32 Products { get; set; }

        /// <summary>版本数</summary>
        Int32 Versions { get; set; }

        /// <summary>成员数。主要成员</summary>
        Int32 Members { get; set; }

        /// <summary>协助成员数。其它团队临时协助该团队的成员</summary>
        Int32 AssistMembers { get; set; }

        /// <summary>机器人</summary>
        String WebHook { get; set; }

        /// <summary>备注</summary>
        String Remark { get; set; }
        #endregion
    }
}