using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using NewLife.Reflection;

namespace Zero.Data.Projects;

/// <summary>版本计划</summary>
public partial class VersionPlanModel : IModel
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
                "TeamId" => TeamId,
                "ProductId" => ProductId,
                "Name" => Name,
                "Kind" => Kind,
                "StartDate" => StartDate,
                "EndDate" => EndDate,
                "ManHours" => ManHours,
                "Enable" => Enable,
                "Completed" => Completed,
                "Stories" => Stories,
                "Remark" => Remark,
                _ => this.GetValue(name, false),
            };
        }
        set
        {
            switch (name)
            {
                case "ID": ID = value.ToInt(); break;
                case "TeamId": TeamId = value.ToInt(); break;
                case "ProductId": ProductId = value.ToInt(); break;
                case "Name": Name = Convert.ToString(value); break;
                case "Kind": Kind = Convert.ToString(value); break;
                case "StartDate": StartDate = value.ToDateTime(); break;
                case "EndDate": EndDate = value.ToDateTime(); break;
                case "ManHours": ManHours = value.ToInt(); break;
                case "Enable": Enable = value.ToBoolean(); break;
                case "Completed": Completed = value.ToBoolean(); break;
                case "Stories": Stories = value.ToInt(); break;
                case "Remark": Remark = Convert.ToString(value); break;
                default: this.SetValue(name, value); break;
            }
        }
    }
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
