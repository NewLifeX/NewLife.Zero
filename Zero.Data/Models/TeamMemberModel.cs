using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>团队成员。每个团队拥有哪些成员，每个成员有一个主力团队</summary>
    public partial class TeamMemberModel : ITeamMember
    {
        #region 属性
        /// <summary>编号</summary>
        public Int32 ID { get; set; }

        /// <summary>团队</summary>
        public Int32 TeamId { get; set; }

        /// <summary>成员</summary>
        public Int32 MemberId { get; set; }

        /// <summary>类型</summary>
        public String Kind { get; set; }

        /// <summary>主要。是否该成员的主要团队</summary>
        public Boolean Major { get; set; }

        /// <summary>组长。该团队组长</summary>
        public Boolean Leader { get; set; }

        /// <summary>启用</summary>
        public Boolean Enable { get; set; }

        /// <summary>备注</summary>
        public String Remark { get; set; }
        #endregion

        #region 拷贝
        /// <summary>拷贝模型对象</summary>
        /// <param name="model">模型</param>
        public void Copy(ITeamMember model)
        {
            ID = model.ID;
            TeamId = model.TeamId;
            MemberId = model.MemberId;
            Kind = model.Kind;
            Major = model.Major;
            Leader = model.Leader;
            Enable = model.Enable;
            Remark = model.Remark;
        }
        #endregion
    }
}