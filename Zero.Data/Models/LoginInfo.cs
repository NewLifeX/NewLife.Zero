using NewLife.Remoting.Models;

namespace Zero.Data.Models;

/// <summary>节点登录信息</summary>
public class LoginInfo : LoginRequest
{
    #region 属性
    /// <summary>产品编码</summary>
    public String ProductCode { get; set; }

    /// <summary>名称。可用于标识设备的名称</summary>
    public String Name { get; set; }

    /// <summary>系统名</summary>
    public String OSName { get; set; }

    /// <summary>系统版本</summary>
    public String OSVersion { get; set; }

    /// <summary>处理器架构</summary>
    public String Architecture { get; set; }

    /// <summary>机器名</summary>
    public String MachineName { get; set; }

    /// <summary>用户名</summary>
    public String UserName { get; set; }

    /// <summary>核心数</summary>
    public Int32 ProcessorCount { get; set; }

    /// <summary>内存大小</summary>
    public UInt64 Memory { get; set; }

    /// <summary>磁盘大小。应用所在盘</summary>
    public UInt64 TotalSize { get; set; }
    #endregion
}