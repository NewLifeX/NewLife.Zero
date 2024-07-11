using System;
using NewLife.Agent;
using NewLife.Log;
using NewLife.Threading;
using Stardust;
using XCode;
using Zero.Data.Projects;

// 比较传统的Windows服务应用模版。优先推荐Console+StarAgent

namespace Zero.Agent;

internal class Program
{
    private static void Main(String[] args) => new MyServices().Main(args);
}

/// <summary>代理服务例子。自定义服务程序可参照该类实现。</summary>
public class MyServices : ServiceBase
{
    #region 属性
    /// <summary>性能跟踪器</summary>
    public ITracer Tracer { get; set; }

    private StarFactory _star;
    #endregion

    #region 构造函数
    /// <summary>实例化一个代理服务</summary>
    public MyServices()
    {
        // 一般在构造函数里面指定服务名
        ServiceName = "XAgent";

        DisplayName = "新生命服务代理";
        Description = "用于承载各种服务的服务代理！";
    }
    #endregion

    #region 核心
    private TimerX _timer;
    private TimerX _timer2;
    private TimerX _timer3;
    /// <summary>开始工作</summary>
    /// <param name="reason"></param>
    public override void StartWork(String reason)
    {
        WriteLog("业务开始……");

        // 配置星尘。自动读取配置文件 config/star.config 中的服务器地址、应用标识、密钥
        _star = new StarFactory();
        Tracer = _star.Tracer;

        // 从配置中心读取参数设置
        var period1 = _star.Config["Period_Work1"].ToInt();
        var period2 = _star.Config["Period_Work2"].ToInt();
        if (period1 == 0) period1 = 60;
        if (period2 == 0) period2 = 24 * 3600;
        var cron3 = _star.Config["Cron_Work3"];
        if (cron3 == null) cron3 = "*/30 * 9-17 * * 1-5";

        // 5秒开始，每60秒执行一次
        _timer = new TimerX(DoWork1, null, 5_000, period1 * 000) { Async = true };
        // 每天凌晨2点13分执行一次
        _timer2 = new TimerX(DoWork2, null, DateTime.Today.AddMinutes(2 * 60 + 13), period2 * 1000) { Async = true };
        // 工作日朝九晚五每半分钟
        _timer3 = new TimerX(DoWork3, null, cron3) { Async = true };

        base.StartWork(reason);
    }

    private void DoWork1(Object state)
    {
        // 简易型埋点，测量调用次数和耗时，跟内部HttpClient和数据库操作形成上下级调用链，并送往星尘监控中心
        using var span = Tracer?.NewSpan("work1");

        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 日志会输出到当前目录的Log子目录中
        XTrace.WriteLine($"Work1代码执行时间：{time}");
    }

    private void DoWork2(Object state)
    {
        // 完整型埋点，增加测量错误次数和明细
        using var span = Tracer?.NewSpan("work2");

        var time = DateTime.Now;
        try
        {
            // 业务处理，清理已经停用的项目成员关系
            var list = Member.FindAll(Member._.Enable == false);
            foreach (var item in list)
            {
                var tms = TeamMember.FindAllByMemberId(item.ID);
                foreach (var elm in tms)
                {
                    elm.Enable = false;
                }
                tms.Save();
            }
        }
        catch (Exception ex)
        {
            // 记录错误以及上下文，一起送往星尘监控中心
            span?.SetError(ex, time);

            throw;
        }
        finally
        {
            span?.Dispose();
        }
    }

    private void DoWork3(Object state)
    {
        // 简易型埋点，测量调用次数和耗时，跟内部HttpClient和数据库操作形成上下级调用链，并送往星尘监控中心
        using var span = Tracer?.NewSpan("work3");

        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 日志会输出到当前目录的Log子目录中
        XTrace.WriteLine($"Work3代码执行时间：{time}");
    }

    /// <summary>停止服务</summary>
    /// <param name="reason"></param>
    public override void StopWork(String reason)
    {
        WriteLog("业务结束！{0}", reason);

        _timer.Dispose();
        _timer2.Dispose();
        _timer3.Dispose();

        base.StopWork(reason);
    }
    #endregion
}