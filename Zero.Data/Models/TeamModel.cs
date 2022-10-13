using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>团队。管理一系列相关的产品和应用系统</summary>
    public partial class TeamModel : ITeam
    {
        #region 属性
        /// <summary>编号</summary>
        public Int32 ID { get; set; }

        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>编码</summary>
        public String Code { get; set; }

        /// <summary>组长</summary>
        public Int32 LeaderId { get; set; }

        /// <summary>启用</summary>
        public Boolean Enable { get; set; }

        /// <summary>产品数</summary>
        public Int32 Products { get; set; }

        /// <summary>版本数</summary>
        public Int32 Versions { get; set; }

        /// <summary>成员数。主要成员</summary>
        public Int32 Members { get; set; }

        /// <summary>协助成员数。其它团队临时协助该团队的成员</summary>
        public Int32 AssistMembers { get; set; }

        /// <summary>机器人</summary>
        public String WebHook { get; set; }

        /// <summary>备注</summary>
        public String Remark { get; set; }
        #endregion

        #region 拷贝
        /// <summary>拷贝模型对象</summary>
        /// <param name="model">模型</param>
        public void Copy(ITeam model)
        {
            ID = model.ID;
            Name = model.Name;
            Code = model.Code;
            LeaderId = model.LeaderId;
            Enable = model.Enable;
            Products = model.Products;
            Versions = model.Versions;
            Members = model.Members;
            AssistMembers = model.AssistMembers;
            WebHook = model.WebHook;
            Remark = model.Remark;
        }
        #endregion
    }
}