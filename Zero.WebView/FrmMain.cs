namespace Zero.WebView;

public partial class FrmMain : Form
{
    public FrmMain()
    {
        InitializeComponent();
        InitializeWebView2();
    }

    private async void InitializeWebView2()
    {
        try
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.Navigate("https://www.bing.com");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to initialize WebView2: " + ex.Message);
        }
    }
}
