using System.Text;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;
using NewLife.Web;
using XCode;
using XCode.Membership;
using Zero.Data.Common;
using Zero.Data.Projects;

namespace Zero.Web.Areas.Projects.Controllers
{
    [ProjectsArea]
    [Menu(90)]
    public class TeamController : EntityController<Team>
    {
        static TeamController() => LogOnChange = true;

        protected override IEnumerable<Team> Search(Pager p)
        {
            var enable = p["enable"]?.ToBoolean();

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            p.RetrieveState = true;

            return Team.Search(null, null, enable, start, end, p["Q"], p);
        }

        protected override Int32 OnInsert(Team entity)
        {
            var rs = base.OnInsert(entity);

            JoinTeam(entity);
            entity.Update();

            return rs;
        }

        protected override Int32 OnUpdate(Team entity)
        {
            var et = entity as IEntity;
            if (et.Dirtys[nameof(entity.LeaderId)]) JoinTeam(entity);

            // 修正成员数、产品数和版本数
            entity.Refresh();

            return base.OnUpdate(entity);
        }

        private void JoinTeam(Team entity)
        {
            var list = TeamMember.FindAllByTeamId(entity.ID);

            // 原组长下岗
            var old = list.FirstOrDefault(e => e.Leader);
            if (old != null)
            {
                old.Leader = false;
                old.Update();
            }

            // 更换组长
            if (entity.LeaderId > 0)
            {
                // 新组长上岗
                var tm = list.FirstOrDefault(e => e.MemberId == entity.LeaderId);
                if (tm == null) list.Add(tm = new TeamMember { TeamId = entity.ID, MemberId = entity.LeaderId });
                tm.Major = true;
                tm.Leader = true;
                tm.Enable = true;
                tm.Save();

                // 更新团队成员数
                entity.Members = list.Count;

                // 这个人不再属于其它团队的主成员
                var list2 = TeamMember.FindAllByMemberId(entity.LeaderId);
                foreach (var item in list2)
                {
                    if (item.TeamId != entity.ID) item.Major = false;
                }
                list2.Update();
            }
        }

        /// <summary>批量刷新</summary>
        /// <returns></returns>
        [EntityAuthorize(PermissionFlags.Update)]
        public ActionResult Refresh()
        {
            var count = 0;
            var ids = GetRequest("keys").SplitAsInt();
            if (ids.Length > 0)
            {
                foreach (var id in ids)
                {
                    var team = Team.FindByID(id);
                    if (team != null)
                    {
                        team.Refresh();
                        if (team.Update() != 0) count++;

                        // 机器人
                        var robot = RobotHelper.CreateRobot(team);
                        if (robot != null)
                        {
                            var prds = Product.FindAllByTeamId(team.ID).Where(e => e.Enable).ToList();
                            var members = TeamMember.FindAllByTeamId(team.ID).Where(e => e.Enable).OrderBy(e => e.MemberName).ToList();
                            var versions = VersionPlan.FindAllNotCompleted(team.ID, -1).OrderBy(e => e.StartDate).ToList();
                            var uri = Request.GetRawUrl();
                            uri = new Uri(uri, "/Projects/Product?teamId=" + team.ID);

                            var sb = new StringBuilder();
                            sb.AppendLine($"### [{team}]团队报告");
                            sb.AppendLine($">组长：<font color=\"#FF0033\">{team.Leader}</font>");
                            sb.AppendLine($">产品：<font color=\"#6600CC\">{prds.Join()}</font>");
                            sb.AppendLine($">成员：<font color=\"info\">{members.Where(e => e.Major).Join(",", e => e.MemberName)}</font>");
                            sb.AppendLine($">协助：<font color=\"info\">{members.Where(e => !e.Major).Join(",", e => e.MemberName)}</font>");

                            if (versions.Count > 0)
                            {
                                sb.AppendLine($">版本：");
                                foreach (var item in versions)
                                {
                                    // 过期、未开始、正常
                                    var color = item.EndDate.Date < DateTime.Today
                                        ? "#FF0000"
                                        : item.StartDate.Date > DateTime.Today
                                            ? "#00CC00"
                                            : "#3366CC";
                                    sb.AppendLine($">\t<font color=\"{color}\">[{item.Product}] {item} ({item.StartDate:MM/dd} - {item.EndDate:MM/dd})</font>");
                                }

                                var storis = Story.Search(versions.Select(e => e.ID).ToArray());
                                if (storis.Count > 0)
                                {
                                    sb.AppendLine($">故事：");
                                    foreach (var item in storis)
                                    {
                                        sb.AppendLine($">\t[{item.Product}/{item.Version}] {item} [{item.Member}] ({item.StartDate:MM/dd} - {item.EndDate:MM/dd})");
                                    }
                                }
                            }

                            sb.AppendLine($"[更多信息]({uri})");

                            robot.SendMarkDown(sb.ToString());
                        }
                    }
                }
            }

            return JsonRefresh($"共刷新[{count}]个团队");
        }
    }
}