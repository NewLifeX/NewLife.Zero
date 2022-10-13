using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>成员。所有可用团队成员</summary>
    public partial class MemberDto : IMember
    {
        #region 属性
        /// <summary>编号</summary>
        public Int32 ID { get; set; }

        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>类型</summary>
        public String Kind { get; set; }

        /// <summary>团队。所属主团队</summary>
        public Int32 TeamId { get; set; }

        /// <summary>启用</summary>
        public Boolean Enable { get; set; }

        /// <summary>团队数。所在团队总数，含协助团队</summary>
        public Int32 Teams { get; set; }

        /// <summary>用户。所属登录用户</summary>
        public Int32 UserId { get; set; }

        /// <summary>用户名</summary>
        public String UserName { get; set; }

        /// <summary>备注</summary>
        public String Remark { get; set; }
        #endregion

        #region 拷贝
        /// <summary>拷贝模型对象</summary>
        /// <param name="model">模型</param>
        public void Copy(IMember model)
        {
            ID = model.ID;
            Name = model.Name;
            Kind = model.Kind;
            TeamId = model.TeamId;
            Enable = model.Enable;
            Teams = model.Teams;
            UserId = model.UserId;
            UserName = model.UserName;
            Remark = model.Remark;
        }
        #endregion
    }
}