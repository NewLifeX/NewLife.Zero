using Microsoft.AspNetCore.Mvc;
using Zero.Data.Nodes;
using NewLife;
using NewLife.Cube;
using NewLife.Cube.Extensions;
using NewLife.Cube.ViewModels;
using NewLife.Log;
using NewLife.Web;
using XCode.Membership;
using static Zero.Data.Nodes.Node;

namespace Zero.Web.Areas.Nodes.Controllers;

/// <summary>节点</summary>
[Menu(30, true, Icon = "fa-table")]
[NodesArea]
public class NodeController : NodeEntityController<Node>
{
    static NodeController()
    {
        LogOnChange = true;

        //ListFields.RemoveField("Id", "Creator");
        //ListFields.RemoveCreateField().RemoveRemarkField();
        var list = ListFields;
        list.Clear();
        var allows = new[] { "ID", "Name", "Code", "Category", "ProductCode", "CityName", "Enable", "Version", "OSKind", "IP", "OS", "MachineName", "Cpu", "Memory", "TotalSize", "Logins", "LastActive", "OnlineTime", "UpdateTime", "UpdateIP" };
        foreach (var item in allows)
            list.AddListField(item);

        {
            var df = ListFields.GetField("Name") as ListField;
            df.Url = "/Nodes/Node/Detail?id={Id}";
            df.Target = "_blank";
        }
        {
            var df = ListFields.AddListField("Log", "UpdateTime");
            df.DisplayName = "日志";
            df.Url = "/Admin/Log?category=节点&linkId={Id}";
            df.Target = "_frame";
        }
    }

    /// <summary>高级搜索。列表页查询、导出Excel、导出Json、分享页等使用</summary>
    /// <param name="p">分页器。包含分页排序参数，以及Http请求参数</param>
    /// <returns></returns>
    protected override IEnumerable<Node> Search(Pager p)
    {
        var nodeId = p["Id"].ToInt(-1);
        if (nodeId > 0)
        {
            var node = Node.FindByKey(nodeId);
            if (node != null) return [node];
        }

        var rids = p["areaId"].SplitAsInt("/");
        var provinceId = rids.Length > 0 ? rids[0] : -1;
        var cityId = rids.Length > 1 ? rids[1] : -1;

        var category = p["category"];
        var product = p["product"];
        var version = p["version"];
        var enable = p["enable"]?.ToBoolean();

        var start = p["dtStart"].ToDateTime();
        var end = p["dtEnd"].ToDateTime();

        return Node.Search(provinceId, cityId, category, product, version, enable, start, end, p["Q"], p);
    }
}