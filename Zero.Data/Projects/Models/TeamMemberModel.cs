using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Data;
using NewLife.Reflection;

namespace Zero.Data.Projects;

/// <summary>团队成员。每个团队拥有哪些成员，每个成员有一个主力团队</summary>
public partial class TeamMemberModel : IModel
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
                "MemberId" => MemberId,
                "Kind" => Kind,
                "Major" => Major,
                "Leader" => Leader,
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
                case "TeamId": TeamId = value.ToInt(); break;
                case "MemberId": MemberId = value.ToInt(); break;
                case "Kind": Kind = Convert.ToString(value); break;
                case "Major": Major = value.ToBoolean(); break;
                case "Leader": Leader = value.ToBoolean(); break;
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
