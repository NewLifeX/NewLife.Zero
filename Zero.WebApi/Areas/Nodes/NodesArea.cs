using System.ComponentModel;
using NewLife;
using NewLife.Cube;

namespace Zero.Web.Areas.Nodes
{
    /// <summary>节点管理</summary>
    [DisplayName("节点管理")]
    public class NodesArea : AreaBase
    {
        public NodesArea() : base(nameof(NodesArea).TrimEnd("Area")) { }
    }
}