using System;
using System.ComponentModel;
using NewLife.Configuration;

namespace Zero.Server
{
    /// <summary>配置</summary>
    [Config("Stardust")]
    public class Setting : Config<Setting>
    {
        #region 属性
        /// <summary>APM监控服务器</summary>
        [Description("APM监控服务器")]
        public String TracerServer { get; set; } = "http://star.newlifex.com:6600";
        #endregion
    }
}
