using System.Xml.Serialization;

namespace Zero.Web;

/// <summary>应用服务信息</summary>
public class ServiceItem
{
    #region 属性
    /// <summary>名称。全局唯一，默认应用名</summary>
    [XmlAttribute]
    public String Name { get; set; }

    /// <summary>Url地址</summary>
    [XmlAttribute]
    public String Url { get; set; }

    /// <summary>延迟时间。默认0ms</summary>
    [XmlAttribute]
    public Int32 Delay { get; set; }
    #endregion
}