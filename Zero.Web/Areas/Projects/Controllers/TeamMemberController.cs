using System;
using System.Collections.Generic;
using System.Reflection;
using NewLife.Cube;
using NewLife.Web;
using Zero.Data.Projects;
using XCode;
using XCode.Membership;

namespace Zero.Web.Areas.Projects.Controllers
{
    [ProjectsArea]
    public class TeamMemberController : EntityController<TeamMember>
    {
        protected override IEnumerable<TeamMember> Search(Pager p)
        {
            var teamId = p["teamId"].ToInt(-1);
            var memberId = p["memberId"].ToInt(-1);
            var enable = p["enable"]?.ToBoolean();

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            return TeamMember.Search(teamId, memberId, enable, start, end, p["Q"], p);
        }

        /// <summary>菜单不可见</summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        protected override IDictionary<MethodInfo, Int32> ScanActionMenu(IMenu menu)
        {
            if (menu.Visible)
            {
                menu.Visible = false;
                (menu as IEntity).Update();
            }

            return base.ScanActionMenu(menu);
        }

        protected override Int32 OnInsert(TeamMember entity)
        {
            var rs = base.OnInsert(entity);

            entity.Team?.Fix();
            entity.Member?.Fix();

            return rs;
        }
    }
}