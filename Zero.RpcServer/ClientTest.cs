using NewLife.Log;
using NewLife.Remoting;
using NewLife.Security;
using NewLife.Serialization;

namespace Zero.RpcServer;

static class ClientTest
{
    /// <summary>Tcp连接ApiServer</summary>
    public static async void TcpTest(Int32 port)
    {
        await Task.Delay(1_000);
        XTrace.WriteLine("");
        XTrace.WriteLine("Tcp开始连接！");

        // 连接服务端
        var client = new ApiClient($"tcp://127.0.0.2:{port}");
        client.Name = "小Tcp";

        await Process(client);

        // 关闭连接
        client.Close("测试完成");
    }

    /// <summary>Udp连接ApiServer</summary>
    public static async void UdpTest(Int32 port)
    {
        await Task.Delay(2_000);
        XTrace.WriteLine("");
        XTrace.WriteLine("Udp开始连接！");

        // 连接服务端
        //var client = new ApiClient($"udp://127.0.0.3:{port}");
        // UDP协议使用127.0.0.3地址后，服务端响应时发往127.0.0.1，导致客户端无法匹配接收
        var client = new ApiClient($"udp://127.0.0.1:{port}");
        client.Name = "小Udp";

        await Process(client);

        // 关闭连接
        client.Close("测试完成");
    }

    /// <summary>Tcp连接ApiServer</summary>
    public static async void WebSocketTest(Int32 port)
    {
        await Task.Delay(3_000);
        XTrace.WriteLine("");
        XTrace.WriteLine("WebSocket开始连接！");

        // 连接服务端
        var client = new ApiClient($"ws://127.0.0.4:{port}");
        client.Name = "小Ws";

        await Process(client);

        // 关闭连接
        client.Close("测试完成");
    }

    static async Task Process(ApiClient client)
    {
        try
        {
            client.Log = XTrace.Log;
#if DEBUG
            client.EncoderLog = XTrace.Log;
            client.SocketLog = XTrace.Log;
#endif
            client.Open();

            // 获取所有接口
            client.WriteLog("获取所有接口");
            var apis = await client.InvokeAsync<String[]>("api/all");
            client.WriteLog("共有接口数：{0}", apis.Length);

            // 获取服务端信息
            client.WriteLog("获取服务端信息");
            var state = Rand.NextString(8);
            var state2 = Rand.NextString(8);
            var infs = await client.InvokeAsync<IDictionary<String, Object>>("api/info", new { state, state2 });
            client.WriteLog("服务端信息：{0}", infs.ToJson(true));
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);
        }
    }

    /// <summary>Http连接ApiServer</summary>
    public static async void HttpTest(Int32 port)
    {
        await Task.Delay(4_000);
        XTrace.WriteLine("");
        XTrace.WriteLine("Http开始连接！");

        // 连接服务端
        var client = new ApiHttpClient($"http://127.0.0.2:{port}");
        client.Log = XTrace.Log;

        var apis = await client.GetAsync<String[]>("api/all");
        client.WriteLog("共有接口数：{0}", apis.Length);

        var state = Rand.NextString(8);
        var state2 = Rand.NextString(8);
        var infs = await client.PostAsync<IDictionary<String, Object>>("api/info", new { state, state2 });
        client.WriteLog("服务端信息：{0}", infs.ToJson(true));
    }
}
