using System.Text;
using Gtk;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using Stardust;
using XCode;
using XCode.Membership;
using Node = Zero.Data.Nodes.Node;

namespace Zero.GtkForm;

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

        XTrace.UseConsole();
        MachineInfo.RegisterAsync();

        StartClient();

        GLib.ExceptionManager.UnhandledException += ExceptionManager_UnhandledException;

        // 预热数据层，执行自动建表等操作
        Task.Run(() =>
        {
            var dal = User.Meta.Session.Dal;
            dal = Node.Meta.Session.Dal;
            _ = EntityFactory.InitAllAsync();
        });

        Application.Init();

        var app = new Application(_factory.AppId, GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var win = new MainWindow();
        app.AddWindow(win);

        win.Show();
        Application.Run();
    }

    private static void ExceptionManager_UnhandledException(GLib.UnhandledExceptionArgs args)
    {
        if (args.ExceptionObject is Exception ex) XTrace.WriteException(ex);
    }

    static StarFactory _factory;
    static StarClient _Client;
    private static void StartClient()
    {
        var set = ClientSetting.Current;
        var server = set.Server;
        if (server.IsNullOrEmpty()) return;

        XTrace.WriteLine("初始化服务端地址：{0}", server);

        _factory = new StarFactory(server, "ZeroGtk", null)
        {
            Log = XTrace.Log,
        };

        var client = new StarClient(server)
        {
            Code = set.Code,
            Secret = set.Secret,
            ProductCode = "ZeroGtk",
            Setting = set,

            Tracer = _factory.Tracer,
            Log = XTrace.Log,
        };

        client.Open();

        Host.RegisterExit(() => client.Logout("ApplicationExit"));

        _Client = client;
    }
}