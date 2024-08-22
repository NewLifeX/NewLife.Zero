using System.ComponentModel;
using AntJob;
using Zero.AntJob.Services;

namespace Zero.AntJob.Jobs;

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
        job.DataTime = DateTime.Now; // 开始时间
        job.Step = 24 * 60 * 60; // 一天执行一次
        _testService = testService;
    }

    public override Int32 Execute(JobContext ctx) => _testService.TestInt();
}