using AntJob;
using AntJob.Extensions;
using NewLife.Security;
using XCode;
using Zero.Data.Projects;

namespace Zero.AntJob.Jobs;

/// <summary>跟踪产品表数据，只要有新增或修改，马上为其创建版本计划</summary>
internal class BuildPlan : DataHandler
{
    public BuildPlan()
    {
        var job = Job;
        job.DataTime = DateTime.Today;
        job.Step = 30;
    }

    public override Boolean Start()
    {
        // 指定要抽取数据的实体类以及时间字段
        Factory = Product.Meta.Factory;
        Field = Product._.UpdateTime;

        return base.Start();
    }

    protected override Boolean ProcessItem(JobContext ctx, IEntity entity)
    {
        var pi = entity as Product;

        var days = Rand.Next(100);
        var startDate = pi.UpdateTime.Date;
        var endDate = startDate.AddDays(days);

        // 创建版本计划信息
        var will = new VersionPlan
        {
            TeamId = pi.TeamId,
            ProductId = pi.ID,
            Name = "v" + new Version(Rand.Next(10), Rand.Next(100)),

            StartDate = startDate,
            EndDate = endDate,
            ManHours = Rand.Next(100) * 8,
            Enable = Rand.Next(2) == 1,

            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now,
        };

        will.Insert();

        return true;
    }
}