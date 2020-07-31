using NewLife.Cube;
using Zero.Data.Projects;
using Zero.Web.Areas.Projects;

namespace Zero.Web.Areas.Project.Controllers
{
    [ProjectArea]
    public class TeamController : EntityController<Team>
    {
        static TeamController() => MenuOrder = 90;
    }
}