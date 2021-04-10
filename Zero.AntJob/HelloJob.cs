using System;
using AntJob;
using NewLife;
using NewLife.Configuration;

namespace Zero.AntJob
{
    internal class HelloJob : Handler
    {
        private readonly IConfigProvider _config;

        public HelloJob(IConfigProvider config)
        {
            // 今天零点开始，每10秒一次
            var job = Job;
            job.Start = DateTime.Today;
            job.Step = 10;

            _config = config;
        }

        protected override Int32 Execute(JobContext ctx)
        {
            var title = _config["Title"];
            if (title.IsNullOrEmpty()) title = "新生命系统";

            // 当前任务时间
            var time = ctx.Task.Start;
            WriteLog("{1}！当前任务时间：{0}", time, title);

            // 成功处理数据量
            return 1;
        }
    }
}