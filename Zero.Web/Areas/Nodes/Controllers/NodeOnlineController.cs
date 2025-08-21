using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;
using NewLife.Remoting.Models;
using NewLife.Remoting.Services;
using NewLife.Web;
using XCode.Membership;
using Zero.Data.Nodes;

namespace Zero.Web.Areas.Nodes.Controllers;

/// <summary>节点在线</summary>
[Menu(20, true, Icon = "fa-table")]
[NodesArea]
public class NodeOnlineController : NodeEntityController<NodeOnline>
{
    private readonly IDeviceService _deviceService;

    static NodeOnlineController()
    {
        //LogOnChange = true;

        ListFields.RemoveField("ProvinceName", "Macs", "Token");
        ListFields.RemoveCreateField().RemoveRemarkField();

        //{
        //    var df = ListFields.GetField("Code") as ListField;
        //    df.Url = "?code={Code}";
        //}
        //{
        //    var df = ListFields.AddListField("devices", null, "Onlines");
        //    df.DisplayName = "查看设备";
        //    df.Url = "Device?groupId={Id}";
        //    df.DataVisible = e => (e as NodeOnline).Devices > 0;
        //}
        //{
        //    var df = ListFields.GetField("Kind") as ListField;
        //    df.GetValue = e => ((Int32)(e as NodeOnline).Kind).ToString("X4");
        //}
        //ListFields.TraceUrl("TraceId");
    }

    public NodeOnlineController(IDeviceService deviceService) => _deviceService = deviceService;

    /// <summary>高级搜索。列表页查询、导出Excel、导出Json、分享页等使用</summary>
    /// <param name="p">分页器。包含分页排序参数，以及Http请求参数</param>
    /// <returns></returns>
    protected override IEnumerable<NodeOnline> Search(Pager p)
    {
        var nodeId = p["nodeId"].ToInt(-1);
        var rids = p["areaId"].SplitAsInt("/");
        var provinceId = rids.Length > 0 ? rids[0] : -1;
        var cityId = rids.Length > 1 ? rids[1] : -1;
        var category = p["category"];

        var start = p["dtStart"].ToDateTime();
        var end = p["dtEnd"].ToDateTime();

        return NodeOnline.Search(nodeId, provinceId, cityId, category, start, end, p["Q"], p);
    }

    [DisplayName("检查更新")]
    [EntityAuthorize((PermissionFlags)16)]
    public async Task<ActionResult> CheckUpgrade()
    {
        var ts = new List<Task>();
        foreach (var item in SelectKeys)
        {
            var online = NodeOnline.FindById(item.ToInt());
            if (online?.Node != null)
            {
                var cmd = new CommandModel
                {
                    Command = "node/upgrade",
                    Expire = DateTime.UtcNow.AddSeconds(600),
                };
                ts.Add(_deviceService.SendCommand(online.Node, cmd, HttpContext.RequestAborted));
            }
        }

        await Task.WhenAll(ts);

        return JsonRefresh("操作成功！");
    }

    [DisplayName("执行命令")]
    [EntityAuthorize((PermissionFlags)16)]
    public async Task<ActionResult> Execute(String command, String argument)
    {
        if (GetRequest("keys") == null) throw new ArgumentNullException(nameof(SelectKeys));
        if (command.IsNullOrEmpty()) throw new ArgumentNullException(nameof(command));

        var ts = new List<Task<Int32>>();
        foreach (var item in SelectKeys)
        {
            var online = NodeOnline.FindById(item.ToInt());
            if (online != null && online.Node != null)
            {
                var cmd = new CommandModel
                {
                    Command = command,
                    Argument = argument,
                    Expire = DateTime.UtcNow.AddSeconds(30),
                };
                ts.Add(_deviceService.SendCommand(online.Node, cmd, HttpContext.RequestAborted));
            }
        }

        var rs = await Task.WhenAll(ts);

        return JsonRefresh($"操作成功！下发指令{rs.Length}个，成功{rs.Count(e => e > 0)}个");
    }
}