﻿using AntJob;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using Zero.AntJob.Jobs;

namespace Zero.AntJob;

/// <summary>蚂蚁调度后台工作者，负责装配并启动调度作业程序</summary>
public class AntJobWorker : BackgroundService
{
    private Scheduler _scheduler;
    private readonly IServiceProvider _serviceProvider;
    private readonly AntSetting _setting;

    public AntJobWorker(IServiceProvider serviceProvider, AntSetting setting)
    {
        _serviceProvider = serviceProvider;
        _setting = setting;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 实例化调度器
        var scheduler = new Scheduler
        {
            ServiceProvider = _serviceProvider,
            Log = XTrace.Log,
        };

        scheduler.Join(_setting);

        // 添加作业
        scheduler.AddHandler<HelloJob>();
        scheduler.AddHandler<TestJob>();
        scheduler.AddHandler<BuildProduct>();
        scheduler.AddHandler<BuildPlan>();

        // 启动调度引擎，调度器内部多线程处理
        scheduler.StartAsync();
        _scheduler = scheduler;

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _scheduler.TryDispose();
        _scheduler = null;

        return Task.CompletedTask;
    }
}