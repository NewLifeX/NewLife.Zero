using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using NewLife.Reflection;

namespace Zero.Data.Projects;

/// <summary>故事。用户故事的目标是将特定价值提供给客户，不必是传统意义上的外部最终用户，也可以是依赖您团队的组织内部客户或同事。用户故事是简单语言中的几句话，概述了所需的结果。</summary>
public partial class StoryModel : IModel
{
    #region 属性
    /// <summary>编号</summary>
    public Int32 ID { get; set; }

    /// <summary>产品</summary>
    public Int32 ProductId { get; set; }

    /// <summary>版本</summary>
    public Int32 VersionId { get; set; }

    /// <summary>处理人</summary>
    public Int32 MemberId { get; set; }

    /// <summary>事项</summary>
    public String Title { get; set; }

    /// <summary>开始日期</summary>
    public DateTime StartDate { get; set; }

    /// <summary>结束日期</summary>
    public DateTime EndDate { get; set; }

    /// <summary>工时</summary>
    public Int32 ManHours { get; set; }

    /// <summary>启用</summary>
    public Boolean Enable { get; set; }

    /// <summary>备注</summary>
    public String Remark { get; set; }
    #endregion

    #region 获取/设置 字段值
    /// <summary>获取/设置 字段值</summary>
    /// <param name="name">字段名</param>
    /// <returns></returns>
    public virtual Object this[String name]
    {
        get
        {
            return name switch
            {
                "ID" => ID,
                "ProductId" => ProductId,
                "VersionId" => VersionId,
                "MemberId" => MemberId,
                "Title" => Title,
                "StartDate" => StartDate,
                "EndDate" => EndDate,
                "ManHours" => ManHours,
                "Enable" => Enable,
                "Remark" => Remark,
                _ => this.GetValue(name, false),
            };
        }
        set
        {
            switch (name)
            {
                case "ID": ID = value.ToInt(); break;
                case "ProductId": ProductId = value.ToInt(); break;
                case "VersionId": VersionId = value.ToInt(); break;
                case "MemberId": MemberId = value.ToInt(); break;
                case "Title": Title = Convert.ToString(value); break;
                case "StartDate": StartDate = value.ToDateTime(); break;
                case "EndDate": EndDate = value.ToDateTime(); break;
                case "ManHours": ManHours = value.ToInt(); break;
                case "Enable": Enable = value.ToBoolean(); break;
                case "Remark": Remark = Convert.ToString(value); break;
                default: this.SetValue(name, value); break;
            }
        }
    }
    #endregion

    #region 拷贝
    /// <summary>拷贝模型对象</summary>
    /// <param name="model">模型</param>
    public void Copy(IStory model)
    {
        ID = model.ID;
        ProductId = model.ProductId;
        VersionId = model.VersionId;
        MemberId = model.MemberId;
        Title = model.Title;
        StartDate = model.StartDate;
        EndDate = model.EndDate;
        ManHours = model.ManHours;
        Enable = model.Enable;
        Remark = model.Remark;
    }
    #endregion
}
