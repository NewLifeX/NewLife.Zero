using NewLife;
using NewLife.Data;

namespace Zero.RpcServer;

/// <summary>自定义控制器。包含多个服务</summary>
/// <remarks>
/// 控制器规范：
/// 1，控制器类不要求公开可见，也无需继承任何基类，需要明码注册到服务端
/// 2，控制器类中的方法不要求公开可见，但方法名和参数名必须固定，跟客户端一致
/// 3，第一段路径名为控制器名（去掉Controller），第二段路径名为方法名，如/Area/FindByID
/// 4，通过[Api]特性指定控制器名和方法名
/// </remarks>
internal class MyController
{
    /// <summary>添加，标准业务服务，走Json序列化</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Int32 Add(Int32 x, Int32 y) => x + y;

    /// <summary>RC4加解密，高速业务服务，二进制收发不经序列化</summary>
    /// <param name="pk"></param>
    /// <returns></returns>
    public IPacket RC4(IPacket pk)
    {
        var data = pk.ToArray();
        var pass = "NewLife".GetBytes();

        return (ArrayPacket)data.RC4(pass);
    }
}