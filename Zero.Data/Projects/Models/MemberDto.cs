using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using NewLife.Reflection;

namespace Zero.Data.Projects;

/// <summary>成员。所有可用团队成员</summary>
public partial class MemberDto : IMember, IModel
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
                "Kind" => Kind,
                "TeamId" => TeamId,
                "Enable" => Enable,
                "Teams" => Teams,
                "UserId" => UserId,
                "UserName" => UserName,
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
                case "Kind": Kind = Convert.ToString(value); break;
                case "TeamId": TeamId = value.ToInt(); break;
                case "Enable": Enable = value.ToBoolean(); break;
                case "Teams": Teams = value.ToInt(); break;
                case "UserId": UserId = value.ToInt(); break;
                case "UserName": UserName = Convert.ToString(value); break;
                case "Remark": Remark = Convert.ToString(value); break;
                default: this.SetValue(name, value); break;
            }
        }
    }
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
