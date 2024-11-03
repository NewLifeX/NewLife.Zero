using System.ComponentModel;
using NewLife.Configuration;
using NewLife.Remoting.Clients;

namespace ZeroClient;

/// <summary>配置</summary>
[Config("ZeroClient")]
public class ClientSetting : Config<ClientSetting>, IClientSetting
{
    #region 属性
    /// <summary>服务端地址。IoT服务平台地址</summary>
    [Description("服务端地址。IoT服务平台地址")]
    public String Server { get; set; } = "http://localhost:6080";

    /// <summary>客户端编码</summary>
    [Description("客户端编码")]
    public String Code { get; set; }

    /// <summary>客户端密钥</summary>
    [Description("客户端密钥")]
    public String Secret { get; set; }
    #endregion
}