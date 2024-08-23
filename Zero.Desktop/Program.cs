using System.Text;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using Stardust;
using XCode;
using XCode.Membership;
using Zero.Data.Nodes;

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

        XTrace.UseWinForm();
        MachineInfo.RegisterAsync();

        StartClient();

        var set = ClientSetting.Current;

        // 启用语音提示
        StringHelper.EnableSpeechTip = set.SpeechTip;

        if (set.IsNew) "学无先后达者为师，欢迎使用新生命零代客户端！".SpeechTip();

        // 预热数据层，执行自动建表等操作
        Task.Run(() =>
        {
            var dal = User.Meta.Session.Dal;
            dal = Node.Meta.Session.Dal;
            _ = EntityFactory.InitAllAsync();
        });

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        //Application.EnableVisualStyles();
        //Application.SetCompatibleTextRenderingDefault(false);
        //Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.Run(new FrmMain());
    }

    static StarFactory _factory;
    static StarClient _Client;
    private static void StartClient()
    {
        var set = ClientSetting.Current;
        var server = set.Server;
        if (server.IsNullOrEmpty()) return;

        XTrace.WriteLine("初始化服务端地址：{0}", server);

        _factory = new StarFactory(server, "ZeroDesktop", null)
        {
            Log = XTrace.Log,
        };

        var client = new StarClient(server)
        {
            Code = set.Code,
            Secret = set.Secret,
            ProductCode = "ZeroDesktop",
            Setting = set,

            Tracer = _factory.Tracer,
            Log = XTrace.Log,
        };

        client.Open();

        Host.RegisterExit(() => client.Logout("ApplicationExit"));

        _Client = client;
    }
}