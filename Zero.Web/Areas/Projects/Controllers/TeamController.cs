using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;
using NewLife.Web;
using Zero.Data.Projects;
using XCode;
using XCode.Membership;

namespace Zero.Web.Areas.Projects.Controllers
{
    [ProjectsArea]
    public class TeamController : EntityController<Team>
    {
        static TeamController() => MenuOrder = 90;

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
                    }
                }
            }

            return JsonRefresh("共刷新[{0}]个团队".F(count));
        }
    }
}