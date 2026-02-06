using Microsoft.AspNetCore.Mvc;
using NewLife.Remoting.Extensions;

namespace Zero.WebApi.Controllers;

/// <summary>设备控制器</summary>
/// <param name="serviceProvider"></param>
[ApiFilter]
[ApiController]
[Route("[controller]")]
public class NodeController(IServiceProvider serviceProvider) : BaseDeviceController(serviceProvider)
{
}