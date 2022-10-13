using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>产品</summary>
    public partial class ProductModel : IProduct
    {
        #region 属性
        /// <summary>编号</summary>
        public Int32 ID { get; set; }

        /// <summary>团队</summary>
        public Int32 TeamId { get; set; }

        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>类型</summary>
        public String Kind { get; set; }

        /// <summary>负责人</summary>
        public Int32 LeaderId { get; set; }

        /// <summary>启用</summary>
        public Boolean Enable { get; set; }

        /// <summary>版本数</summary>
        public Int32 Versions { get; set; }

        /// <summary>故事数</summary>
        public Int32 Stories { get; set; }

        /// <summary>完成</summary>
        public Boolean Completed { get; set; }

        /// <summary>备注</summary>
        public String Remark { get; set; }
        #endregion

        #region 拷贝
        /// <summary>拷贝模型对象</summary>
        /// <param name="model">模型</param>
        public void Copy(IProduct model)
        {
            ID = model.ID;
            TeamId = model.TeamId;
            Name = model.Name;
            Kind = model.Kind;
            LeaderId = model.LeaderId;
            Enable = model.Enable;
            Versions = model.Versions;
            Stories = model.Stories;
            Completed = model.Completed;
            Remark = model.Remark;
        }
        #endregion
    }
}