using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using NewLife.Reflection;

namespace Zero.Data.Projects;

/// <summary>产品</summary>
public partial class ProductModel : IModel
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
                "Name" => Name,
                "Kind" => Kind,
                "LeaderId" => LeaderId,
                "Enable" => Enable,
                "Versions" => Versions,
                "Stories" => Stories,
                "Completed" => Completed,
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
                case "Name": Name = Convert.ToString(value); break;
                case "Kind": Kind = Convert.ToString(value); break;
                case "LeaderId": LeaderId = value.ToInt(); break;
                case "Enable": Enable = value.ToBoolean(); break;
                case "Versions": Versions = value.ToInt(); break;
                case "Stories": Stories = value.ToInt(); break;
                case "Completed": Completed = value.ToBoolean(); break;
                case "Remark": Remark = Convert.ToString(value); break;
                default: this.SetValue(name, value); break;
            }
        }
    }
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
