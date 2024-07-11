using Zero.Data.Nodes;
using NewLife;
using NewLife.Cube;
using NewLife.Cube.Extensions;
using NewLife.Cube.ViewModels;
using NewLife.Web;
using XCode.Membership;

namespace Zero.Web.Areas.Nodes.Controllers
{
    /// <summary>节点历史</summary>
    [Menu(10, true, Icon = "fa-table")]
    [NodesArea]
    public class NodeHistoryController : EntityController<NodeHistory>
    {
        static NodeHistoryController()
        {
            //LogOnChange = true;

            //ListFields.RemoveField("Id", "Creator");
            ListFields.RemoveCreateField();

        }

        /// <summary>高级搜索。列表页查询、导出Excel、导出Json、分享页等使用</summary>
        /// <param name="p">分页器。包含分页排序参数，以及Http请求参数</param>
        /// <returns></returns>
        protected override IEnumerable<NodeHistory> Search(Pager p)
        {
            //var deviceId = p["deviceId"].ToInt(-1);

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            return NodeHistory.Search(start, end, p["Q"], p);
        }
    }
}