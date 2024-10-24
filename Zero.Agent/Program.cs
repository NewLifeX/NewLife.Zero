using System;

// 比较传统的Windows/Linux服务应用模版。优先推荐Console+StarAgent

namespace Zero.Agent;

internal class Program
{
    private static void Main(String[] args) => new MyServices().Main(args);
}
