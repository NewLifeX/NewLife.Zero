using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Zero.Data.Projects
{
    /// <summary>版本计划</summary>
    public partial class VersionPlanModel : IVersionPlan
    {
        #region 属性
        /// <summary>编号</summary>
        public Int32 ID { get; set; }

        /// <summary>团队</summary>
        public Int32 TeamId { get; set; }

        /// <summary>产品</summary>
        public Int32 ProductId { get; set; }

        /// <summary>名称。版本号</summary>
        public String Name { get; set; }

        /// <summary>类型</summary>
        public String Kind { get; set; }

        /// <summary>开始日期</summary>
        public DateTime StartDate { get; set; }

        /// <summary>结束日期</summary>
        public DateTime EndDate { get; set; }

        /// <summary>工时</summary>
        public Int32 ManHours { get; set; }

        /// <summary>启用</summary>
        public Boolean Enable { get; set; }

        /// <summary>完成</summary>
        public Boolean Completed { get; set; }

        /// <summary>故事数</summary>
        public Int32 Stories { get; set; }

        /// <summary>备注</summary>
        public String Remark { get; set; }
        #endregion

        #region 拷贝
        /// <summary>拷贝模型对象</summary>
        /// <param name="model">模型</param>
        public void Copy(IVersionPlan model)
        {
            ID = model.ID;
            TeamId = model.TeamId;
            ProductId = model.ProductId;
            Name = model.Name;
            Kind = model.Kind;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            ManHours = model.ManHours;
            Enable = model.Enable;
            Completed = model.Completed;
            Stories = model.Stories;
            Remark = model.Remark;
        }
        #endregion
    }
}