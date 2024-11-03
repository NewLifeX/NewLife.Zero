using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Filters;
using NewLife;
using NewLife.Cube;
using NewLife.Cube.ViewModels;
using XCode;

namespace Zero.Web.Areas.Nodes;

[DisplayName("节点管理")]
public class NodesArea : AreaBase
{
    public NodesArea() : base(nameof(NodesArea).TrimEnd("Area")) { }
}

/// <summary>节点管理控制器基类。抽象共性能力</summary>
/// <typeparam name="TEntity"></typeparam>
public abstract class NodeEntityController<TEntity> : EntityController<TEntity> where TEntity : Entity<TEntity>, new()
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        base.OnActionExecuting(filterContext);

        var nodeId = GetRequest("Id").ToInt(-1);
        if (nodeId <= 0) nodeId = GetRequest("nodeId").ToInt(-1);
        if (nodeId > 0)
        {
            PageSetting.NavView = "_Node_Nav";
            PageSetting.EnableNavbar = false;
        }
    }

    protected override FieldCollection OnGetFields(ViewKinds kind, Object model)
    {
        var fields = base.OnGetFields(kind, model);

        if (kind == ViewKinds.List)
        {
            var nodeId = GetRequest("nodeId").ToInt(-1);
            if (nodeId > 0) fields.RemoveField("NodeName");
        }

        return fields;
    }
}