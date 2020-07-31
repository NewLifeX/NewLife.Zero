using System;
using System.Collections.Generic;
using NewLife.Cube;
using NewLife.Web;
using Zero.Data.Projects;
using Zero.Web.Areas.Projects;

namespace Zero.Web.Areas.Project.Controllers
{
    [ProjectArea]
    public class MemberController : EntityController<Member>
    {
        static MemberController() => MenuOrder = 60;

        protected override IEnumerable<Member> Search(Pager p)
        {
            var teamId = p["teamId"].ToInt(-1);
            var kind = p["kind"];

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            p.RetrieveState = true;

            return Member.Search(teamId, kind, start, end, p["Q"], p);
        }
    }
}