using System;
using NewLife.Security;

namespace Zero.AntJob2.Services
{
    public interface ITestService
    {
        string TestString();

        Int32 TestInt();
    }

    public class TestService : ITestService
    {
        public Int32 TestInt() => Rand.Next(1, 10);

        public String TestString() => DateTime.Now.ToFullString();
    }
}
