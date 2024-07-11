using System.Text;
using NewLife;
using NewLife.Log;
using NewLife.Threading;
using Stardust;
using XCode;

namespace Zero.Desktop;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // 支持GB2312编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if NET5_0_OR_GREATER
        XTrace.UseWinForm();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
#else
        XTrace.UseWinForm();
#endif
        MachineInfo.RegisterAsync();

        StartClient();

        var set = ClientSetting.Current;

        // 启用语音提示
        StringHelper.EnableSpeechTip = set.SpeechTip;

        if (set.IsNew) "学无先后达者为师，欢迎使用新生命零代客户端！".SpeechTip();

        // 预热数据层，执行自动建表等操作
        _ = EntityFactory.InitAllAsync();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new FrmMain());
    }

    static TimerX _timer;
    static StarClient _Client;
    private static void StartClient()
    {
        var set = ClientSetting.Current;
        var server = set.Server;
        if (server.IsNullOrEmpty()) return;

        XTrace.WriteLine("初始化服务端地址：{0}", server);

        var star = new StarFactory();

        var client = new StarClient(server)
        {
            Code = set.Code,
            Secret = set.Secret,
            ProductCode = "ZeroDesktop",
            Setting = set,

            Log = XTrace.Log,
        };

        Application.ApplicationExit += (s, e) => client.Logout("ApplicationExit");

        // 可能需要多次尝试
        client.Open();

        _Client = client;
    }
}