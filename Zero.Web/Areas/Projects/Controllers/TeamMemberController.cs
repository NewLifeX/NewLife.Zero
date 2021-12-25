using System;
using System.Collections.Generic;
using NewLife.Cube;
using NewLife.Web;
using XCode.Membership;
using Zero.Data.Projects;

namespace Zero.Web.Areas.Projects.Controllers
{
    [Menu(0, false)]
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

        protected override Int32 OnInsert(TeamMember entity)
        {
            var rs = base.OnInsert(entity);

            entity.Team?.Fix();
            entity.Member?.Fix();

            return rs;
        }
    }
}