using System;
using NewLife;
using NewLife.Agent;
using NewLife.Log;
using NewLife.Remoting;
using NewLife.Threading;
using Stardust.Monitors;
using XCode;
using XCode.DataAccessLayer;
using Zero.Data.Projects;

namespace Zero.Agent
{
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
        /// <summary>开始工作</summary>
        /// <param name="reason"></param>
        protected override void StartWork(String reason)
        {
            WriteLog("业务开始……");

            // 配置APM性能跟踪器
            var set = Setting.Current;
            if (!set.TracerServer.IsNullOrEmpty())
            {
                // 配置指向星尘监控中心
                var tracer = new StarTracer(set.TracerServer) { Log = XTrace.Log };
                DefaultTracer.Instance = tracer;
                ApiHelper.Tracer = tracer;
                DAL.GlobalTracer = tracer;
                Tracer = tracer;
            }

            // 5秒开始，每60秒执行一次
            _timer = new TimerX(DoWork1, null, 5_000, 60_000) { Async = true };
            // 每天凌晨2点13分执行一次
            _timer2 = new TimerX(DoWork2, null, DateTime.Today.AddMinutes(2 * 60 + 13), 24 * 3600 * 1000) { Async = true };

            base.StartWork(reason);
        }

        private void DoWork1(Object state)
        {
            // 简易型埋点，测量调用次数和耗时，跟内部HttpClient和数据库操作形成上下级调用链，并送往星尘监控中心
            using var span = Tracer?.NewSpan("work1");

            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 日志会输出到当前目录的Log子目录中
            XTrace.WriteLine($"代码执行时间：{time}");
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

        /// <summary>停止服务</summary>
        /// <param name="reason"></param>
        protected override void StopWork(String reason)
        {
            WriteLog("业务结束！{0}", reason);

            _timer.Dispose();
            _timer2.Dispose();

            base.StopWork(reason);
        }
        #endregion
    }
}