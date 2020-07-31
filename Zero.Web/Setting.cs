using System;
using System.ComponentModel;
using NewLife.Configuration;

namespace Zero.Web
{
    /// <summary>配置</summary>
    [Config("Stardust")]
    public class Setting : Config<Setting>
    {
        #region 属性
        /// <summary>调试开关。默认true</summary>
        [Description("调试开关。默认true")]
        public Boolean Debug { get; set; } = true;

        /// <summary>APM监控服务器。</summary>
        [Description("APM监控服务器。")]
        public String TracerServer { get; set; }
        #endregion

        #region 构造
        #endregion
    }
}