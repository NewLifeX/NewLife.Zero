using Microsoft.AspNetCore.Mvc;
using Zero.Data.Nodes;
using NewLife;
using NewLife.Cube;
using NewLife.Cube.Extensions;
using NewLife.Cube.ViewModels;
using NewLife.Log;
using NewLife.Web;
using XCode.Membership;
using static Zero.Data.Nodes.NodeHistory;
using Zero.Web.Areas.Nodes;

namespace Zero.Web.Areas.Nodes.Controllers;

/// <summary>节点历史</summary>
[Menu(10, true, Icon = "fa-table")]
[NodesArea]
public class NodeHistoryController : NodeEntityController<NodeHistory>
{
    static NodeHistoryController()
    {
        //LogOnChange = true;

        ListFields.RemoveField("ProvinceName", "Compile");
        ListFields.RemoveRemarkField();

        {
            var df = ListFields.AddListField("Remark", "TraceId");
            df.TextAlign = TextAligns.Nowrap;
        }

        ListFields.TraceUrl();
    }

    /// <summary>高级搜索。列表页查询、导出Excel、导出Json、分享页等使用</summary>
    /// <param name="p">分页器。包含分页排序参数，以及Http请求参数</param>
    /// <returns></returns>
    protected override IEnumerable<NodeHistory> Search(Pager p)
    {
        var rids = p["areaId"].SplitAsInt("/");
        var provinceId = rids.Length > 0 ? rids[0] : -1;
        var cityId = rids.Length > 1 ? rids[1] : -1;

        var nodeId = p["nodeId"].ToInt(-1);
        var action = p["action"];
        var success = p["success"]?.ToBoolean();

        var start = p["dtStart"].ToDateTime();
        var end = p["dtEnd"].ToDateTime();

        return NodeHistory.Search(nodeId, provinceId, cityId, action, success, start, end, p["Q"], p);
    }
}