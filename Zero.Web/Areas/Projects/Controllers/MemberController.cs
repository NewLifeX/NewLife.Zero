using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Web;
using XCode;
using XCode.Membership;
using Zero.Data.Projects;

namespace Zero.Web.Areas.Projects.Controllers
{
    [ProjectsArea]
    [Menu(80)]
    public class MemberController : EntityController<Member>
    {
        static MemberController() => LogOnChange = true;

        protected override IEnumerable<Member> Search(Pager p)
        {
            var teamId = p["teamId"].ToInt(-1);
            var kind = p["kind"];

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            return Member.Search(teamId, kind, start, end, p["Q"], p);
        }

        protected override Int32 OnInsert(Member entity)
        {
            var rs = base.OnInsert(entity);

            // 先添加成员，拿到ID，加入团队
            JoinTeam(entity);
            entity.Update();

            return rs;
        }

        protected override Int32 OnUpdate(Member entity)
        {
            // 加入团队
            JoinTeam(entity);

            return base.OnUpdate(entity);
        }

        private void JoinTeam(Member entity)
        {
            if (entity.TeamId > 0)
            {
                var list = TeamMember.FindAllByMemberId(entity.ID);
                var tm = list.FirstOrDefault(e => e.TeamId == entity.TeamId);
                if (tm == null) list.Add(tm = new TeamMember { TeamId = entity.TeamId, MemberId = entity.ID });

                // 如果这个成员没有主要团队，就选这个吧
                if (!list.Any(e => e.Major)) tm.Major = true;

                tm.Kind = entity.Kind;
                tm.Enable = true;
                //tm.Save();

                // 同步更新关联表
                foreach (var item in list)
                {
                    item.Kind = entity.Kind;
                }
                list.Save();

                // 刷新成员所属团队数
                entity.Teams = list.Count;

                // 刷新团队信息
                tm.Team?.Fix();
            }
        }

        /// <summary>绑定用户</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Update)]
        public ActionResult BindUser(Int32 id)
        {
            var member = Member.FindByID(id);
            if (member == null) throw new ArgumentNullException(nameof(id));

            var user = ManageProvider.User;
            if (user == null) throw new Exception("未登录！");

            member.UserId = user.ID;
            member.UserName = user.Name;
            member.Refresh();
            member.Update();

            return RedirectToAction("Index");
        }
    }
}