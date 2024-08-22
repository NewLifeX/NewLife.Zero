using AntJob;
using NewLife.Caching;
using NewLife.Security;
using XCode;
using Zero.Data.Projects;

namespace Zero.AntJob.Jobs;

/// <summary>定时创建产品</summary>
internal class BuildProduct : Handler
{
    private readonly ICacheProvider _cacheProvider;

    public BuildProduct(ICacheProvider cacheProvider)
    {
        var job = Job;
        job.DataTime = DateTime.Today;
        job.Step = 15;

        _cacheProvider = cacheProvider;
    }

    public override Int32 Execute(JobContext ctx)
    {
        // 随机
        var count = Rand.Next(1, 9);

        var list = new List<Product>();
        for (var i = 0; i < count; i++)
        {
            var pi = new Product
            {
                TeamId = 1,
                Name = Rand.NextString(8),
                Enable = Rand.Next(2) == 1,

                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
            };

            // Redis去重
            if (!_cacheProvider.Cache.Add($"product:{pi.Name}", DateTime.Now)) pi.Name = Rand.NextString(8);

            list.Add(pi);
        }
        list.Insert(true);

        // 成功处理数据量
        return count;
    }
}