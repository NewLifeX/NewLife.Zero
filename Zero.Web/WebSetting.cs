﻿using System.ComponentModel;
using NewLife;
using NewLife.Configuration;
using NewLife.Remoting.Models;
using NewLife.Security;

namespace Zero.Web;

/// <summary>设置</summary>
[Config("Web")]
public class WebSetting : Config<WebSetting>, ITokenSetting
{
    #region 属性
    /// <summary>调试。默认启用</summary>
    [Description("调试。默认启用")]
    public Boolean Debug { get; set; } = true;

    /// <summary>端口。默认1080</summary>
    [Description("端口。默认1080")]
    public Int32 Port { get; set; } = 1080;

    /// <summary>令牌密钥。用于生成JWT令牌的算法和密钥，如HS256:ABCD1234</summary>
    [Description("令牌密钥。用于生成JWT令牌的算法和密钥，如HS256:ABCD1234")]
    public String TokenSecret { get; set; }

    /// <summary>令牌有效期。默认2*3600秒</summary>
    [Description("令牌有效期。默认2*3600秒")]
    public Int32 TokenExpire { get; set; } = 2 * 3600;

    /// <summary>会话超时。默认600秒</summary>
    [Description("会话超时。默认600秒")]
    [Category("设备管理")]
    public Int32 SessionTimeout { get; set; } = 600;

    /// <summary>自动注册。允许客户端自动注册，默认true</summary>
    [Description("自动注册。允许客户端自动注册，默认true")]
    [Category("设备管理")]
    public Boolean AutoRegister { get; set; } = true;

    /// <summary>服务集合</summary>
    [Description("服务集合")]
    public ServiceItem[] Services { get; set; }
    #endregion

    #region 方法
    /// <summary>加载完成后</summary>
    protected override void OnLoaded()
    {
        if (Port == 0) Port = 1080;

        if (TokenSecret.IsNullOrEmpty() || TokenSecret.Split(':').Length != 2) TokenSecret = $"HS256:{Rand.NextString(16)}";

        if (Services == null || Services.Length == 0)
        {
            var si = new ServiceItem
            {
                Name = "test",
                Url = "http://127.0.0.1:8000",
                Delay = 0,
            };
            var si2 = new ServiceItem
            {
                Name = "test2",
                Url = "http://127.0.0.1:8100",
                Delay = 100,
            };

            Services = new[] { si, si2 };
        }

        base.OnLoaded();
    }
    #endregion
}