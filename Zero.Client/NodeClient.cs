using System.Runtime.InteropServices;
using NewLife;
using NewLife.Model;
using NewLife.Remoting.Clients;
using NewLife.Remoting.Models;
using NewLife.Security;
using Zero.Client.Models;
using MigrationEventArgs = Stardust.Models.MigrationEventArgs;

namespace Zero.Client;

/// <summary>节点客户端</summary>
public class NodeClient : ClientBase
{
    #region 属性
    /// <summary>产品编码</summary>
    public String ProductCode { get; set; }

    /// <summary>服务迁移</summary>
    public event EventHandler<MigrationEventArgs> OnMigration;
    #endregion

    #region 构造
    public NodeClient(ClientSetting setting) : base(setting)
    {
        // 设置动作，开启下行通知
        Features = Features.Login | Features.Logout | Features.Ping | Features.Notify | Features.Upgrade;
        SetActions("Node/");
    }
    #endregion

    #region 方法
    protected override void OnInit()
    {
        var provider = ServiceProvider ??= ObjectContainer.Provider;

        PasswordProvider = new SaltPasswordProvider { Algorithm = "md5", SaltTime = 60 };

        // 找到容器，注册默认的模型实现，供后续InvokeAsync时自动创建正确的模型对象
        var container = provider.GetService<IObjectContainer>() ?? ObjectContainer.Current;
        if (container != null)
        {
            container.TryAddTransient<ILoginRequest, LoginInfo>();
            container.TryAddTransient<IPingRequest, PingInfo>();
        }

        base.OnInit();
    }
    #endregion

    #region 登录
    public override ILoginRequest BuildLoginRequest()
    {
        var request = new LoginInfo();
        FillLoginRequest(request);

        var mi = MachineInfo.GetCurrent();
        var path = ".".GetFullPath();
        var driveInfo = DriveInfo.GetDrives().FirstOrDefault(e => path.StartsWithIgnoreCase(e.Name));

        request.ProductCode = ProductCode;
        request.Name = Environment.MachineName;

        request.OSName = mi.OSName;
        request.OSVersion = mi.OSVersion;
        request.Architecture = RuntimeInformation.ProcessArchitecture + "";
        request.MachineName = Environment.MachineName;
        request.UserName = Environment.UserName;

        request.ProcessorCount = Environment.ProcessorCount;
        request.Memory = mi.Memory;
        request.TotalSize = (UInt64)(driveInfo?.TotalSize ?? 0);

        return request;
    }
    #endregion

    #region 心跳
    public override IPingRequest BuildPingRequest()
    {
        var request = new PingInfo();
        FillPingRequest(request);

        return request;
    }

    /// <summary>心跳</summary>
    /// <returns></returns>
    public override async Task<IPingResponse> Ping(CancellationToken cancellationToken = default)
    {
        var rs = await base.Ping(cancellationToken);
        if (rs != null)
        {
            // 迁移到新服务器
            if (rs is PingResponse prs && !prs.NewServer.IsNullOrEmpty() && prs.NewServer != Server)
            {
                var arg = new MigrationEventArgs { NewServer = prs.NewServer };

                OnMigration?.Invoke(this, arg);
                if (!arg.Cancel)
                {
                    if (Features.HasFlag(Features.Logout))
                        await Logout("切换新服务器", cancellationToken);

                    // 清空原有链接，添加新链接
                    Server = prs.NewServer;
                    Client = null;

                    if (Features.HasFlag(Features.Login))
                        await Login(nameof(Ping), cancellationToken);
                }
            }
        }

        return rs;
    }
    #endregion
}