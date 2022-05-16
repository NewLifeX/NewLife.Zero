using NewLife.Security;

namespace Zero.AntJob.Services;

public interface ITestService
{
    String TestString();

    Int32 TestInt();
}

public class TestService : ITestService
{
    public Int32 TestInt() => Rand.Next(1, 10);

    public String TestString() => DateTime.Now.ToFullString();
}