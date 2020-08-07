using System;
using System.ComponentModel;
using NewLife;
using NewLife.Cube;

namespace Zero.Web.Areas.Projects
{
    [DisplayName("项目管理")]
    public class ProjectsArea : AreaBase
    {
        public ProjectsArea() : base(nameof(ProjectsArea).TrimEnd("Area")) { }

        static ProjectsArea() => RegisterArea<ProjectsArea>();
    }
}