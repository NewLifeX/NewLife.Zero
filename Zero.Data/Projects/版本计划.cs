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

/// <summary>版本计划</summary>
[Serializable]
[DataObject]
[Description("版本计划")]
[BindIndex("IX_VersionPlan_TeamId", false, "TeamId")]
[BindIndex("IX_VersionPlan_ProductId", false, "ProductId")]
[BindTable("VersionPlan", Description = "版本计划", ConnName = "Zero", DbType = DatabaseType.None)]
public partial class VersionPlan : IVersionPlan, IEntity<VersionPlanModel>
{
    #region 属性
    private Int32 _ID;
    /// <summary>编号</summary>
    [DisplayName("编号")]
    [Description("编号")]
    [DataObjectField(true, true, false, 0)]
    [BindColumn("ID", "编号", "")]
    public Int32 ID { get => _ID; set { if (OnPropertyChanging("ID", value)) { _ID = value; OnPropertyChanged("ID"); } } }

    private Int32 _TeamId;
    /// <summary>团队</summary>
    [DisplayName("团队")]
    [Description("团队")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("TeamId", "团队", "")]
    public Int32 TeamId { get => _TeamId; set { if (OnPropertyChanging("TeamId", value)) { _TeamId = value; OnPropertyChanged("TeamId"); } } }

    private Int32 _ProductId;
    /// <summary>产品</summary>
    [DisplayName("产品")]
    [Description("产品")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("ProductId", "产品", "")]
    public Int32 ProductId { get => _ProductId; set { if (OnPropertyChanging("ProductId", value)) { _ProductId = value; OnPropertyChanged("ProductId"); } } }

    private String _Name;
    /// <summary>名称。版本号</summary>
    [DisplayName("名称")]
    [Description("名称。版本号")]
    [DataObjectField(false, false, false, 50)]
    [BindColumn("Name", "名称。版本号", "", Master = true)]
    public String Name { get => _Name; set { if (OnPropertyChanging("Name", value)) { _Name = value; OnPropertyChanged("Name"); } } }

    private String _Kind;
    /// <summary>类型</summary>
    [DisplayName("类型")]
    [Description("类型")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("Kind", "类型", "")]
    public String Kind { get => _Kind; set { if (OnPropertyChanging("Kind", value)) { _Kind = value; OnPropertyChanged("Kind"); } } }

    private DateTime _StartDate;
    /// <summary>开始日期</summary>
    [DisplayName("开始日期")]
    [Description("开始日期")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("StartDate", "开始日期", "")]
    public DateTime StartDate { get => _StartDate; set { if (OnPropertyChanging("StartDate", value)) { _StartDate = value; OnPropertyChanged("StartDate"); } } }

    private DateTime _EndDate;
    /// <summary>结束日期</summary>
    [DisplayName("结束日期")]
    [Description("结束日期")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("EndDate", "结束日期", "")]
    public DateTime EndDate { get => _EndDate; set { if (OnPropertyChanging("EndDate", value)) { _EndDate = value; OnPropertyChanged("EndDate"); } } }

    private Int32 _ManHours;
    /// <summary>工时</summary>
    [DisplayName("工时")]
    [Description("工时")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("ManHours", "工时", "")]
    public Int32 ManHours { get => _ManHours; set { if (OnPropertyChanging("ManHours", value)) { _ManHours = value; OnPropertyChanged("ManHours"); } } }

    private Boolean _Enable;
    /// <summary>启用</summary>
    [DisplayName("启用")]
    [Description("启用")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Enable", "启用", "")]
    public Boolean Enable { get => _Enable; set { if (OnPropertyChanging("Enable", value)) { _Enable = value; OnPropertyChanged("Enable"); } } }

    private Boolean _Completed;
    /// <summary>完成</summary>
    [DisplayName("完成")]
    [Description("完成")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Completed", "完成", "")]
    public Boolean Completed { get => _Completed; set { if (OnPropertyChanging("Completed", value)) { _Completed = value; OnPropertyChanged("Completed"); } } }

    private Int32 _Stories;
    /// <summary>故事数</summary>
    [DisplayName("故事数")]
    [Description("故事数")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Stories", "故事数", "")]
    public Int32 Stories { get => _Stories; set { if (OnPropertyChanging("Stories", value)) { _Stories = value; OnPropertyChanged("Stories"); } } }

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
    public void Copy(VersionPlanModel model)
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

    #region 获取/设置 字段值
    /// <summary>获取/设置 字段值</summary>
    /// <param name="name">字段名</param>
    /// <returns></returns>
    public override Object this[String name]
    {
        get => name switch
        {
            "ID" => _ID,
            "TeamId" => _TeamId,
            "ProductId" => _ProductId,
            "Name" => _Name,
            "Kind" => _Kind,
            "StartDate" => _StartDate,
            "EndDate" => _EndDate,
            "ManHours" => _ManHours,
            "Enable" => _Enable,
            "Completed" => _Completed,
            "Stories" => _Stories,
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
                case "TeamId": _TeamId = value.ToInt(); break;
                case "ProductId": _ProductId = value.ToInt(); break;
                case "Name": _Name = Convert.ToString(value); break;
                case "Kind": _Kind = Convert.ToString(value); break;
                case "StartDate": _StartDate = value.ToDateTime(); break;
                case "EndDate": _EndDate = value.ToDateTime(); break;
                case "ManHours": _ManHours = value.ToInt(); break;
                case "Enable": _Enable = value.ToBoolean(); break;
                case "Completed": _Completed = value.ToBoolean(); break;
                case "Stories": _Stories = value.ToInt(); break;
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
    /// <summary>取得版本计划字段信息的快捷方式</summary>
    public partial class _
    {
        /// <summary>编号</summary>
        public static readonly Field ID = FindByName("ID");

        /// <summary>团队</summary>
        public static readonly Field TeamId = FindByName("TeamId");

        /// <summary>产品</summary>
        public static readonly Field ProductId = FindByName("ProductId");

        /// <summary>名称。版本号</summary>
        public static readonly Field Name = FindByName("Name");

        /// <summary>类型</summary>
        public static readonly Field Kind = FindByName("Kind");

        /// <summary>开始日期</summary>
        public static readonly Field StartDate = FindByName("StartDate");

        /// <summary>结束日期</summary>
        public static readonly Field EndDate = FindByName("EndDate");

        /// <summary>工时</summary>
        public static readonly Field ManHours = FindByName("ManHours");

        /// <summary>启用</summary>
        public static readonly Field Enable = FindByName("Enable");

        /// <summary>完成</summary>
        public static readonly Field Completed = FindByName("Completed");

        /// <summary>故事数</summary>
        public static readonly Field Stories = FindByName("Stories");

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

    /// <summary>取得版本计划字段名称的快捷方式</summary>
    public partial class __
    {
        /// <summary>编号</summary>
        public const String ID = "ID";

        /// <summary>团队</summary>
        public const String TeamId = "TeamId";

        /// <summary>产品</summary>
        public const String ProductId = "ProductId";

        /// <summary>名称。版本号</summary>
        public const String Name = "Name";

        /// <summary>类型</summary>
        public const String Kind = "Kind";

        /// <summary>开始日期</summary>
        public const String StartDate = "StartDate";

        /// <summary>结束日期</summary>
        public const String EndDate = "EndDate";

        /// <summary>工时</summary>
        public const String ManHours = "ManHours";

        /// <summary>启用</summary>
        public const String Enable = "Enable";

        /// <summary>完成</summary>
        public const String Completed = "Completed";

        /// <summary>故事数</summary>
        public const String Stories = "Stories";

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
