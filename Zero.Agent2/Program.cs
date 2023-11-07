using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife.Extensions.Hosting.AgentService;
using NewLife.Log;
using Zero.Agent2;

// 使用系统服务的Worker模版。优先推荐Console+StarAgent

XTrace.UseConsole();

Environment.CurrentDirectory = ".".GetFullPath();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddStardust();

        services.AddHostedService<Worker>();
    })
    //.UseWindowsService(options =>
    //{
    //    options.ServiceName = "TestWorker";
    //})
    .UseAgentService(options =>
    {
        options.ServiceName = "ZeroAgent";
        options.DisplayName = "Worker服务测试";
        options.Description = "Worker服务的测试应用";
    })
    .Build();

await host.RunAsync();
