using System;
using System.ComponentModel;
using NewLife;
using NewLife.Cube;

namespace Zero.Web.Areas.Projects
{
    [DisplayName("项目管理")]
    public class ProjectArea : AreaBase
    {
        public ProjectArea() : base(nameof(ProjectArea).TrimEnd("Area")) { }

        static ProjectArea() => RegisterArea<ProjectArea>();
    }
}