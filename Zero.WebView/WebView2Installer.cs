using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Zero.WebView;

internal static class WebView2Installer
{
    //Any CPU
    private static readonly String WebView2OnlineInstallerUrl = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";
    private static readonly String WebView2OnlineInstallerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EdgeWebView2OnlineInstaller.exe");

    //X86
    private static readonly String WebView2X86OfflineInstallerUrl = "https://go.microsoft.com/fwlink/?linkid=2099617";
    private static readonly String WebView2X86OfflineInstallerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EdgeWebView2OfflineInstallerX86.exe");

    //X64
    private static readonly String WebView2X64OfflineInstallerUrl = "https://go.microsoft.com/fwlink/?linkid=2124701";
    private static readonly String WebView2X64OfflineInstallerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EdgeWebView2OfflineInstallerX64.exe");

    //ARM64
    private static readonly String WebView2Arm64OfflineInstallerUrl = "https://go.microsoft.com/fwlink/?linkid=2099616";
    private static readonly String WebView2Arm64OfflineInstallerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EdgeWebView2OfflineInstallerArm64.exe");

    public static async Task InstallWebView2IfNeededAsync(Boolean usingOnlineInstaller = true, Boolean useSilentInstall = true)
    {
        // WebView2 is already installed
        if (IsWebView2Installed())
            return;

        // Confirm download url and installer path
        (var webView2RuntimeUrl, var webView2InstallerPath) = GetWebView2InstallerUrlAndPath(usingOnlineInstaller);

        // Check if the installer already exists
        if (!File.Exists(webView2InstallerPath))
            await DownloadWebView2InstallerAsync(webView2RuntimeUrl, webView2InstallerPath).ConfigureAwait(false);

        // Install WebView2
        await InstallWebView2Async(webView2InstallerPath, useSilentInstall).ConfigureAwait(false);
    }

    private static async Task DownloadWebView2InstallerAsync(String webView2RuntimeUrl, String webView2InstallerPath)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(webView2RuntimeUrl);
        response.EnsureSuccessStatusCode();
        await using var fileStream = new FileStream(webView2InstallerPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fileStream);
    }

    private static async Task InstallWebView2Async(String webView2InstallerPath, Boolean useSilentInstall)
    {
        var tcs = new TaskCompletionSource<Boolean>();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = webView2InstallerPath,
                Arguments = useSilentInstall ? "/silent /install" : "/install",
                CreateNoWindow = false,
                UseShellExecute = true,
                Verb = "runas" // Run as administrator
            },
            EnableRaisingEvents = true
        };
        process.Exited += (_, _) => tcs.SetResult(process.ExitCode == 0);

        process.Start();

        var success = await tcs.Task;
        if (success)
            return;

        var installerLogPath = Path.Combine(Path.GetTempPath(), "msedge_installer.log");
        if (File.Exists(installerLogPath))
        {
            var result =
                MessageBox.Show(
                    $"WebView2 installation failed with exit code: {process.ExitCode}. Do you want to view the log file?",
                    "Installation Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (result == DialogResult.Yes)
                // Open log file
                Process.Start(new ProcessStartInfo
                {
                    FileName = installerLogPath,
                    UseShellExecute = true // Use the system default application to execute
                });
        }
        else
            MessageBox.Show($"WebView2 installation failed with exit code: {process.ExitCode}.",
                "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    private static Boolean IsWebView2Installed()
    {
        try
        {
            var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            return !String.IsNullOrEmpty(version);
        }
        catch (WebView2RuntimeNotFoundException)
        {
            return false;
        }
    }

    private static (String webView2RuntimeUrl, String webView2InstallerPath) GetWebView2InstallerUrlAndPath(Boolean usingOnlineInstaller)
    {
        String webView2RuntimeUrl;
        String webView2InstallerPath;
        if (usingOnlineInstaller)
        {
            webView2RuntimeUrl = WebView2OnlineInstallerUrl;
            webView2InstallerPath = WebView2OnlineInstallerPath;
        }
        else
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    webView2RuntimeUrl = WebView2X86OfflineInstallerUrl;
                    webView2InstallerPath = WebView2X86OfflineInstallerPath;
                    break;
                case Architecture.X64:
                    webView2RuntimeUrl = WebView2X64OfflineInstallerUrl;
                    webView2InstallerPath = WebView2X64OfflineInstallerPath;
                    break;
                case Architecture.Arm64:
                    webView2RuntimeUrl = WebView2Arm64OfflineInstallerUrl;
                    webView2InstallerPath = WebView2Arm64OfflineInstallerPath;
                    break;
                default:
                    throw new PlatformNotSupportedException(RuntimeInformation.ProcessArchitecture + " platform is not supported.");
            }
        return (webView2RuntimeUrl, webView2InstallerPath);
    }
}