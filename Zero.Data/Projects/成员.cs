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

/// <summary>成员。所有可用团队成员</summary>
[Serializable]
[DataObject]
[Description("成员。所有可用团队成员")]
[BindIndex("IU_Member_Name", true, "Name")]
[BindTable("Member", Description = "成员。所有可用团队成员", ConnName = "Zero", DbType = DatabaseType.None)]
public partial class Member : IMember, IEntity<IMember>
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

    private String _Kind;
    /// <summary>类型</summary>
    [DisplayName("类型")]
    [Description("类型")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("Kind", "类型", "")]
    public String Kind { get => _Kind; set { if (OnPropertyChanging("Kind", value)) { _Kind = value; OnPropertyChanged("Kind"); } } }

    private Int32 _TeamId;
    /// <summary>团队。所属主团队</summary>
    [DisplayName("团队")]
    [Description("团队。所属主团队")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("TeamId", "团队。所属主团队", "")]
    public Int32 TeamId { get => _TeamId; set { if (OnPropertyChanging("TeamId", value)) { _TeamId = value; OnPropertyChanged("TeamId"); } } }

    private Boolean _Enable;
    /// <summary>启用</summary>
    [DisplayName("启用")]
    [Description("启用")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Enable", "启用", "")]
    public Boolean Enable { get => _Enable; set { if (OnPropertyChanging("Enable", value)) { _Enable = value; OnPropertyChanged("Enable"); } } }

    private Int32 _Teams;
    /// <summary>团队数。所在团队总数，含协助团队</summary>
    [DisplayName("团队数")]
    [Description("团队数。所在团队总数，含协助团队")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Teams", "团队数。所在团队总数，含协助团队", "")]
    public Int32 Teams { get => _Teams; set { if (OnPropertyChanging("Teams", value)) { _Teams = value; OnPropertyChanged("Teams"); } } }

    private Int32 _UserId;
    /// <summary>用户。所属登录用户</summary>
    [DisplayName("用户")]
    [Description("用户。所属登录用户")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("UserId", "用户。所属登录用户", "")]
    public Int32 UserId { get => _UserId; set { if (OnPropertyChanging("UserId", value)) { _UserId = value; OnPropertyChanged("UserId"); } } }

    private String _UserName;
    /// <summary>用户名</summary>
    [DisplayName("用户名")]
    [Description("用户名")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("UserName", "用户名", "")]
    public String UserName { get => _UserName; set { if (OnPropertyChanging("UserName", value)) { _UserName = value; OnPropertyChanged("UserName"); } } }

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
            "Kind" => _Kind,
            "TeamId" => _TeamId,
            "Enable" => _Enable,
            "Teams" => _Teams,
            "UserId" => _UserId,
            "UserName" => _UserName,
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
                case "Kind": _Kind = Convert.ToString(value); break;
                case "TeamId": _TeamId = value.ToInt(); break;
                case "Enable": _Enable = value.ToBoolean(); break;
                case "Teams": _Teams = value.ToInt(); break;
                case "UserId": _UserId = value.ToInt(); break;
                case "UserName": _UserName = Convert.ToString(value); break;
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
    /// <summary>取得成员字段信息的快捷方式</summary>
    public partial class _
    {
        /// <summary>编号</summary>
        public static readonly Field ID = FindByName("ID");

        /// <summary>名称</summary>
        public static readonly Field Name = FindByName("Name");

        /// <summary>类型</summary>
        public static readonly Field Kind = FindByName("Kind");

        /// <summary>团队。所属主团队</summary>
        public static readonly Field TeamId = FindByName("TeamId");

        /// <summary>启用</summary>
        public static readonly Field Enable = FindByName("Enable");

        /// <summary>团队数。所在团队总数，含协助团队</summary>
        public static readonly Field Teams = FindByName("Teams");

        /// <summary>用户。所属登录用户</summary>
        public static readonly Field UserId = FindByName("UserId");

        /// <summary>用户名</summary>
        public static readonly Field UserName = FindByName("UserName");

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

    /// <summary>取得成员字段名称的快捷方式</summary>
    public partial class __
    {
        /// <summary>编号</summary>
        public const String ID = "ID";

        /// <summary>名称</summary>
        public const String Name = "Name";

        /// <summary>类型</summary>
        public const String Kind = "Kind";

        /// <summary>团队。所属主团队</summary>
        public const String TeamId = "TeamId";

        /// <summary>启用</summary>
        public const String Enable = "Enable";

        /// <summary>团队数。所在团队总数，含协助团队</summary>
        public const String Teams = "Teams";

        /// <summary>用户。所属登录用户</summary>
        public const String UserId = "UserId";

        /// <summary>用户名</summary>
        public const String UserName = "UserName";

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
