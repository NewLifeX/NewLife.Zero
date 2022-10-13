﻿using System.ComponentModel;
using NewLife;
using NewLife.Cube;

namespace Zero.Web.Areas.Nodes
{
    [DisplayName("节点管理")]
    public class NodesArea : AreaBase
    {
        public NodesArea() : base(nameof(NodesArea).TrimEnd("Area")) { }
    }
}