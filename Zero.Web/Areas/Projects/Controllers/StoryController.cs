using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using NewLife.Cube;
using NewLife.Web;
using Zero.Data.Projects;
using XCode;
using XCode.Membership;

namespace Zero.Web.Areas.Projects.Controllers
{
    [ProjectsArea]
    public class StoryController : EntityController<Story>
    {
        protected override IEnumerable<Story> Search(Pager p)
        {
            var productId = p["productId"].ToInt(-1);
            var versionId = p["versionId"].ToInt(-1);
            var memberId = p["memberId"].ToInt(-1);
            var enable = p["enable"]?.ToBoolean();

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            return Story.Search(productId, versionId, memberId, enable, start, end, p["Q"], p);
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

        protected override Boolean Valid(Story entity, DataObjectMethodType type, Boolean post)
        {
            // 默认当前用户作为负责人
            if (!post && type == DataObjectMethodType.Insert)
            {
                var member = Member.FindByUserId(ManageProvider.User.ID);
                if (member != null) entity.MemberId = member.ID;
            }

            return base.Valid(entity, type, post);
        }

        protected override Int32 OnInsert(Story entity)
        {
            var rs = base.OnInsert(entity);

            entity.Product?.Fix();
            entity.Version?.Fix();

            return rs;
        }

        protected override Int32 OnUpdate(Story entity)
        {
            var rs= base.OnUpdate(entity);

            entity.Product?.Fix();
            entity.Version?.Fix();

            return rs;
        }
    }
}