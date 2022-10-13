using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>团队成员。每个团队拥有哪些成员，每个成员有一个主力团队</summary>
    public partial interface ITeamMember
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>团队</summary>
        Int32 TeamId { get; set; }

        /// <summary>成员</summary>
        Int32 MemberId { get; set; }

        /// <summary>类型</summary>
        String Kind { get; set; }

        /// <summary>主要。是否该成员的主要团队</summary>
        Boolean Major { get; set; }

        /// <summary>组长。该团队组长</summary>
        Boolean Leader { get; set; }

        /// <summary>启用</summary>
        Boolean Enable { get; set; }

        /// <summary>备注</summary>
        String Remark { get; set; }
        #endregion
    }
}