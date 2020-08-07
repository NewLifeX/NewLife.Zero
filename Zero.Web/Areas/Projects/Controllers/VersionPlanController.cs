using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;
using NewLife.Web;
using Zero.Data.Projects;
using XCode.Membership;

namespace Zero.Web.Areas.Projects.Controllers
{
    [ProjectsArea]
    public class VersionPlanController : EntityController<VersionPlan>
    {
        static VersionPlanController() => MenuOrder = 60;

        protected override IEnumerable<VersionPlan> Search(Pager p)
        {
            var teamId = p["teamId"].ToInt(-1);
            var productId = p["productId"].ToInt(-1);
            var kind = p["kind"];
            var enable = p["enable"]?.ToBoolean();
            var completed = p["completed"]?.ToBoolean();

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            p.RetrieveState = true;

            return VersionPlan.Search(teamId, productId, kind, enable, completed, start, end, p["Q"], p);
        }

        protected override Int32 OnInsert(VersionPlan entity)
        {
            var rs = base.OnInsert(entity);

            entity.Product?.Fix();
            entity.Team?.Fix();

            return rs;
        }

        protected override Int32 OnUpdate(VersionPlan entity)
        {
            var rs = base.OnUpdate(entity);

            entity.Refresh();
            entity.Product?.Fix();
            entity.Team?.Fix();

            return rs;
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
                    var version = VersionPlan.FindByID(id);
                    if (version != null)
                    {
                        version.Refresh();
                        if (version.Update() != 0) count++;
                    }
                }
            }

            return JsonRefresh("共刷新[{0}]个版本".F(count));
        }
    }
}