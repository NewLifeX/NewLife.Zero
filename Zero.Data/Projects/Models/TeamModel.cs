using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using NewLife.Reflection;

namespace Zero.Data.Projects;

/// <summary>团队。管理一系列相关的产品和应用系统</summary>
public partial class TeamModel : IModel
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
                "Name" => Name,
                "Code" => Code,
                "LeaderId" => LeaderId,
                "Enable" => Enable,
                "Products" => Products,
                "Versions" => Versions,
                "Members" => Members,
                "AssistMembers" => AssistMembers,
                "WebHook" => WebHook,
                "Remark" => Remark,
                _ => this.GetValue(name, false),
            };
        }
        set
        {
            switch (name)
            {
                case "ID": ID = value.ToInt(); break;
                case "Name": Name = Convert.ToString(value); break;
                case "Code": Code = Convert.ToString(value); break;
                case "LeaderId": LeaderId = value.ToInt(); break;
                case "Enable": Enable = value.ToBoolean(); break;
                case "Products": Products = value.ToInt(); break;
                case "Versions": Versions = value.ToInt(); break;
                case "Members": Members = value.ToInt(); break;
                case "AssistMembers": AssistMembers = value.ToInt(); break;
                case "WebHook": WebHook = Convert.ToString(value); break;
                case "Remark": Remark = Convert.ToString(value); break;
                default: this.SetValue(name, value); break;
            }
        }
    }
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
