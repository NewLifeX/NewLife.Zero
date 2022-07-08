using System.ComponentModel;
using NewLife.Configuration;

namespace Zero.Web;

/// <summary>设置</summary>
[Config("Web")]
public class WebSetting : Config<WebSetting>
{
    #region 属性
    /// <summary>调试。默认启用</summary>
    [Description("调试。默认启用")]
    public Boolean Debug { get; set; } = true;

    /// <summary>端口。默认1080</summary>
    [Description("端口。默认1080")]
    public Int32 Port { get; set; } = 1080;

    /// <summary>服务集合</summary>
    [Description("服务集合")]
    public ServiceItem[] Services { get; set; }
    #endregion

    #region 方法
    /// <summary>加载完成后</summary>
    protected override void OnLoaded()
    {
        if (Port == 0) Port = 1080;

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