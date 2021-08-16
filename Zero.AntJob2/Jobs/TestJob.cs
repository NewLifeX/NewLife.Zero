using System;
using System.ComponentModel;
using AntJob;
using Zero.AntJob2.Services;

namespace Zero.AntJob2.Jobs
{
    /// <summary>
    /// TestJob
    /// </summary>
    [DisplayName("Zero.AntJob2.TestJob")]
    [Description("Zero.AntJob2.TestJob")]
    public class TestJob : Handler
    {
        private readonly ITestService _testService;

        public TestJob(ITestService testService)
        {
            var job = Job;
            job.Start = DateTime.Now; // 开始时间
            job.Step = 24 * 60 * 60; // 一天执行一次
            _testService = testService;
        }

        protected override int Execute(JobContext ctx)
        {
            return _testService.TestInt();
        }
    }
}
