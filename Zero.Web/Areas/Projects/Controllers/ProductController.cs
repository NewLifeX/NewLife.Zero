using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Cube;
using NewLife.Web;
using XCode.Membership;
using Zero.Data.Projects;

namespace Zero.Web.Areas.Projects.Controllers
{
    [ProjectsArea]
    [Menu(70)]
    public class ProductController : EntityController<Product>
    {
        static ProductController() => LogOnChange = true;

        protected override IEnumerable<Product> Search(Pager p)
        {
            var teamId = p["teamId"].ToInt(-1);
            var kind = p["kind"];
            var enable = p["enable"]?.ToBoolean();
            var completed = p["completed"]?.ToBoolean();

            var start = p["dtStart"].ToDateTime();
            var end = p["dtEnd"].ToDateTime();

            p.RetrieveState = true;

            return Product.Search(teamId, kind, enable, completed, start, end, p["Q"], p);
        }

        protected override Boolean Valid(Product entity, DataObjectMethodType type, Boolean post)
        {
            // 默认当前用户作为负责人
            if (!post && type == DataObjectMethodType.Insert)
            {
                var member = Member.FindByUserId(ManageProvider.User.ID);
                if (member != null) entity.LeaderId = member.ID;
            }

            return base.Valid(entity, type, post);
        }

        protected override Int32 OnUpdate(Product entity)
        {
            // 修正成员数、产品数和版本数
            entity.Refresh();

            return base.OnUpdate(entity);
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
                    var product = Product.FindByID(id);
                    if (product != null)
                    {
                        product.Refresh();
                        if (product.Update() != 0) count++;
                    }
                }
            }

            return JsonRefresh($"共刷新[{count}]个产品");
        }
    }
}