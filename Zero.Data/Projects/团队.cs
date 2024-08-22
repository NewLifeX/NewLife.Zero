using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife;
using NewLife.Data;
using XCode;
using XCode.Cache;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace Zero.Data.Projects;

/// <summary>团队。管理一系列相关的产品和应用系统</summary>
[Serializable]
[DataObject]
[Description("团队。管理一系列相关的产品和应用系统")]
[BindIndex("IU_Team_Name", true, "Name")]
[BindIndex("IU_Team_Code", true, "Code")]
[BindTable("Team", Description = "团队。管理一系列相关的产品和应用系统", ConnName = "Zero", DbType = DatabaseType.None)]
public partial class Team : ITeam, IEntity<TeamModel>
{
    #region 属性
    private Int32 _ID;
    /// <summary>编号</summary>
    [DisplayName("编号")]
    [Description("编号")]
    [DataObjectField(true, true, false, 0)]
    [BindColumn("ID", "编号", "")]
    public Int32 ID { get => _ID; set { if (OnPropertyChanging("ID", value)) { _ID = value; OnPropertyChanged("ID"); } } }

    private String _Name;
    /// <summary>名称</summary>
    [DisplayName("名称")]
    [Description("名称")]
    [DataObjectField(false, false, false, 50)]
    [BindColumn("Name", "名称", "", Master = true)]
    public String Name { get => _Name; set { if (OnPropertyChanging("Name", value)) { _Name = value; OnPropertyChanged("Name"); } } }

    private String _Code;
    /// <summary>编码</summary>
    [DisplayName("编码")]
    [Description("编码")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("Code", "编码", "")]
    public String Code { get => _Code; set { if (OnPropertyChanging("Code", value)) { _Code = value; OnPropertyChanged("Code"); } } }

    private Int32 _LeaderId;
    /// <summary>组长</summary>
    [DisplayName("组长")]
    [Description("组长")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("LeaderId", "组长", "")]
    public Int32 LeaderId { get => _LeaderId; set { if (OnPropertyChanging("LeaderId", value)) { _LeaderId = value; OnPropertyChanged("LeaderId"); } } }

    private Boolean _Enable;
    /// <summary>启用</summary>
    [DisplayName("启用")]
    [Description("启用")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Enable", "启用", "")]
    public Boolean Enable { get => _Enable; set { if (OnPropertyChanging("Enable", value)) { _Enable = value; OnPropertyChanged("Enable"); } } }

    private Int32 _Products;
    /// <summary>产品数</summary>
    [DisplayName("产品数")]
    [Description("产品数")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Products", "产品数", "")]
    public Int32 Products { get => _Products; set { if (OnPropertyChanging("Products", value)) { _Products = value; OnPropertyChanged("Products"); } } }

    private Int32 _Versions;
    /// <summary>版本数</summary>
    [DisplayName("版本数")]
    [Description("版本数")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Versions", "版本数", "")]
    public Int32 Versions { get => _Versions; set { if (OnPropertyChanging("Versions", value)) { _Versions = value; OnPropertyChanged("Versions"); } } }

    private Int32 _Members;
    /// <summary>成员数。主要成员</summary>
    [DisplayName("成员数")]
    [Description("成员数。主要成员")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Members", "成员数。主要成员", "")]
    public Int32 Members { get => _Members; set { if (OnPropertyChanging("Members", value)) { _Members = value; OnPropertyChanged("Members"); } } }

    private Int32 _AssistMembers;
    /// <summary>协助成员数。其它团队临时协助该团队的成员</summary>
    [DisplayName("协助成员数")]
    [Description("协助成员数。其它团队临时协助该团队的成员")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("AssistMembers", "协助成员数。其它团队临时协助该团队的成员", "")]
    public Int32 AssistMembers { get => _AssistMembers; set { if (OnPropertyChanging("AssistMembers", value)) { _AssistMembers = value; OnPropertyChanged("AssistMembers"); } } }

    private String _WebHook;
    /// <summary>机器人</summary>
    [DisplayName("机器人")]
    [Description("机器人")]
    [DataObjectField(false, false, true, 200)]
    [BindColumn("WebHook", "机器人", "")]
    public String WebHook { get => _WebHook; set { if (OnPropertyChanging("WebHook", value)) { _WebHook = value; OnPropertyChanged("WebHook"); } } }

    private String _CreateUser;
    /// <summary>创建者</summary>
    [DisplayName("创建者")]
    [Description("创建者")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("CreateUser", "创建者", "")]
    public String CreateUser { get => _CreateUser; set { if (OnPropertyChanging("CreateUser", value)) { _CreateUser = value; OnPropertyChanged("CreateUser"); } } }

    private Int32 _CreateUserID;
    /// <summary>创建人</summary>
    [DisplayName("创建人")]
    [Description("创建人")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("CreateUserID", "创建人", "")]
    public Int32 CreateUserID { get => _CreateUserID; set { if (OnPropertyChanging("CreateUserID", value)) { _CreateUserID = value; OnPropertyChanged("CreateUserID"); } } }

    private String _CreateIP;
    /// <summary>创建地址</summary>
    [DisplayName("创建地址")]
    [Description("创建地址")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("CreateIP", "创建地址", "")]
    public String CreateIP { get => _CreateIP; set { if (OnPropertyChanging("CreateIP", value)) { _CreateIP = value; OnPropertyChanged("CreateIP"); } } }

    private DateTime _CreateTime;
    /// <summary>创建时间</summary>
    [DisplayName("创建时间")]
    [Description("创建时间")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("CreateTime", "创建时间", "")]
    public DateTime CreateTime { get => _CreateTime; set { if (OnPropertyChanging("CreateTime", value)) { _CreateTime = value; OnPropertyChanged("CreateTime"); } } }

    private String _UpdateUser;
    /// <summary>更新者</summary>
    [DisplayName("更新者")]
    [Description("更新者")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("UpdateUser", "更新者", "")]
    public String UpdateUser { get => _UpdateUser; set { if (OnPropertyChanging("UpdateUser", value)) { _UpdateUser = value; OnPropertyChanged("UpdateUser"); } } }

    private Int32 _UpdateUserID;
    /// <summary>更新人</summary>
    [DisplayName("更新人")]
    [Description("更新人")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("UpdateUserID", "更新人", "")]
    public Int32 UpdateUserID { get => _UpdateUserID; set { if (OnPropertyChanging("UpdateUserID", value)) { _UpdateUserID = value; OnPropertyChanged("UpdateUserID"); } } }

    private String _UpdateIP;
    /// <summary>更新地址</summary>
    [DisplayName("更新地址")]
    [Description("更新地址")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("UpdateIP", "更新地址", "")]
    public String UpdateIP { get => _UpdateIP; set { if (OnPropertyChanging("UpdateIP", value)) { _UpdateIP = value; OnPropertyChanged("UpdateIP"); } } }

    private DateTime _UpdateTime;
    /// <summary>更新时间</summary>
    [DisplayName("更新时间")]
    [Description("更新时间")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("UpdateTime", "更新时间", "")]
    public DateTime UpdateTime { get => _UpdateTime; set { if (OnPropertyChanging("UpdateTime", value)) { _UpdateTime = value; OnPropertyChanged("UpdateTime"); } } }

    private String _Remark;
    /// <summary>备注</summary>
    [DisplayName("备注")]
    [Description("备注")]
    [DataObjectField(false, false, true, 500)]
    [BindColumn("Remark", "备注", "")]
    public String Remark { get => _Remark; set { if (OnPropertyChanging("Remark", value)) { _Remark = value; OnPropertyChanged("Remark"); } } }
    #endregion

    #region 拷贝
    /// <summary>拷贝模型对象</summary>
    /// <param name="model">模型</param>
    public void Copy(TeamModel model)
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

    #region 获取/设置 字段值
    /// <summary>获取/设置 字段值</summary>
    /// <param name="name">字段名</param>
    /// <returns></returns>
    public override Object this[String name]
    {
        get => name switch
        {
            "ID" => _ID,
            "Name" => _Name,
            "Code" => _Code,
            "LeaderId" => _LeaderId,
            "Enable" => _Enable,
            "Products" => _Products,
            "Versions" => _Versions,
            "Members" => _Members,
            "AssistMembers" => _AssistMembers,
            "WebHook" => _WebHook,
            "CreateUser" => _CreateUser,
            "CreateUserID" => _CreateUserID,
            "CreateIP" => _CreateIP,
            "CreateTime" => _CreateTime,
            "UpdateUser" => _UpdateUser,
            "UpdateUserID" => _UpdateUserID,
            "UpdateIP" => _UpdateIP,
            "UpdateTime" => _UpdateTime,
            "Remark" => _Remark,
            _ => base[name]
        };
        set
        {
            switch (name)
            {
                case "ID": _ID = value.ToInt(); break;
                case "Name": _Name = Convert.ToString(value); break;
                case "Code": _Code = Convert.ToString(value); break;
                case "LeaderId": _LeaderId = value.ToInt(); break;
                case "Enable": _Enable = value.ToBoolean(); break;
                case "Products": _Products = value.ToInt(); break;
                case "Versions": _Versions = value.ToInt(); break;
                case "Members": _Members = value.ToInt(); break;
                case "AssistMembers": _AssistMembers = value.ToInt(); break;
                case "WebHook": _WebHook = Convert.ToString(value); break;
                case "CreateUser": _CreateUser = Convert.ToString(value); break;
                case "CreateUserID": _CreateUserID = value.ToInt(); break;
                case "CreateIP": _CreateIP = Convert.ToString(value); break;
                case "CreateTime": _CreateTime = value.ToDateTime(); break;
                case "UpdateUser": _UpdateUser = Convert.ToString(value); break;
                case "UpdateUserID": _UpdateUserID = value.ToInt(); break;
                case "UpdateIP": _UpdateIP = Convert.ToString(value); break;
                case "UpdateTime": _UpdateTime = value.ToDateTime(); break;
                case "Remark": _Remark = Convert.ToString(value); break;
                default: base[name] = value; break;
            }
        }
    }
    #endregion

    #region 关联映射
    #endregion

    #region 扩展查询
    #endregion

    #region 字段名
    /// <summary>取得团队字段信息的快捷方式</summary>
    public partial class _
    {
        /// <summary>编号</summary>
        public static readonly Field ID = FindByName("ID");

        /// <summary>名称</summary>
        public static readonly Field Name = FindByName("Name");

        /// <summary>编码</summary>
        public static readonly Field Code = FindByName("Code");

        /// <summary>组长</summary>
        public static readonly Field LeaderId = FindByName("LeaderId");

        /// <summary>启用</summary>
        public static readonly Field Enable = FindByName("Enable");

        /// <summary>产品数</summary>
        public static readonly Field Products = FindByName("Products");

        /// <summary>版本数</summary>
        public static readonly Field Versions = FindByName("Versions");

        /// <summary>成员数。主要成员</summary>
        public static readonly Field Members = FindByName("Members");

        /// <summary>协助成员数。其它团队临时协助该团队的成员</summary>
        public static readonly Field AssistMembers = FindByName("AssistMembers");

        /// <summary>机器人</summary>
        public static readonly Field WebHook = FindByName("WebHook");

        /// <summary>创建者</summary>
        public static readonly Field CreateUser = FindByName("CreateUser");

        /// <summary>创建人</summary>
        public static readonly Field CreateUserID = FindByName("CreateUserID");

        /// <summary>创建地址</summary>
        public static readonly Field CreateIP = FindByName("CreateIP");

        /// <summary>创建时间</summary>
        public static readonly Field CreateTime = FindByName("CreateTime");

        /// <summary>更新者</summary>
        public static readonly Field UpdateUser = FindByName("UpdateUser");

        /// <summary>更新人</summary>
        public static readonly Field UpdateUserID = FindByName("UpdateUserID");

        /// <summary>更新地址</summary>
        public static readonly Field UpdateIP = FindByName("UpdateIP");

        /// <summary>更新时间</summary>
        public static readonly Field UpdateTime = FindByName("UpdateTime");

        /// <summary>备注</summary>
        public static readonly Field Remark = FindByName("Remark");

        static Field FindByName(String name) => Meta.Table.FindByName(name);
    }

    /// <summary>取得团队字段名称的快捷方式</summary>
    public partial class __
    {
        /// <summary>编号</summary>
        public const String ID = "ID";

        /// <summary>名称</summary>
        public const String Name = "Name";

        /// <summary>编码</summary>
        public const String Code = "Code";

        /// <summary>组长</summary>
        public const String LeaderId = "LeaderId";

        /// <summary>启用</summary>
        public const String Enable = "Enable";

        /// <summary>产品数</summary>
        public const String Products = "Products";

        /// <summary>版本数</summary>
        public const String Versions = "Versions";

        /// <summary>成员数。主要成员</summary>
        public const String Members = "Members";

        /// <summary>协助成员数。其它团队临时协助该团队的成员</summary>
        public const String AssistMembers = "AssistMembers";

        /// <summary>机器人</summary>
        public const String WebHook = "WebHook";

        /// <summary>创建者</summary>
        public const String CreateUser = "CreateUser";

        /// <summary>创建人</summary>
        public const String CreateUserID = "CreateUserID";

        /// <summary>创建地址</summary>
        public const String CreateIP = "CreateIP";

        /// <summary>创建时间</summary>
        public const String CreateTime = "CreateTime";

        /// <summary>更新者</summary>
        public const String UpdateUser = "UpdateUser";

        /// <summary>更新人</summary>
        public const String UpdateUserID = "UpdateUserID";

        /// <summary>更新地址</summary>
        public const String UpdateIP = "UpdateIP";

        /// <summary>更新时间</summary>
        public const String UpdateTime = "UpdateTime";

        /// <summary>备注</summary>
        public const String Remark = "Remark";
    }
    #endregion
}
