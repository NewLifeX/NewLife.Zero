## NewLife.Zero 零代脚手架
这里展现了NewLife生态圈各种类型项目的模版，开发新项目时可以直接拷贝使用。  

## 公共项目
1. Zero.Core。零代核心库，Web应用、微服务、应用服务、大数据计算和客户端等多个端共用的类库，例如模型类等。原则上要求最少依赖第三方。  
2. Zero.Data。零代数据层，Web应用、微服务、应用服务、大数据计算等共用数据层，主要包括数据层实体类。一般只用于服务端，内有XCode模版及代码生成器。  

## 现代应用
1. Zero.Web。Web管理平台，零代数据管理平台，基于魔方，快速开发各种管理后台  
2. Zero.WebApi。WebApi接口，数据接口，带有Swagger的标准WebApi接口项目  
3. Zero.Worker。系统后台任务，数据处理、定时任务、MQ生产消费、系统监控等超长独立工作的后台任务  

## 网络应用
1. Zero.HttpServer。Http服务，轻量级Http服务器，可用于追求简单Http服务的场景，例如暴露硬件控制为Http接口，如打印组件  
2. Zero.WebSockerServer。WebSocket服务，轻量级WebSocket服务，可用于网页与终端本地硬件保持长连接通信  
3. Zero.RpcServer。RPC服务端，高性能，长连接，数据接口  
4. Zero.TcpServer。Tcp网络服务器，高性能Tcp网络服务器，单机最高400万连接，可用于建立各种协议的网络服务器  

## 经典应用
1. Zero.Agent。系统守护服务，数据处理、定时任务、MQ生产消费、系统监控等超长独立工作的后台任务，常安装为Windows服务，Linux上需要sudo才能安装为守护进程随系统启动  
2. Zero.AntJob。大数据计算，蚂蚁调度系统子程序，编写数据处理等业务逻辑，连接蚂蚁调度中心，拉取作业任务来执行
3. Zero.Console。控制台项目模版
4. Zero.Server。网络服务器项目模版

## 新生命项目矩阵
各项目默认支持net6.0/netstandard2.1/netstandard2.0/net4.61，旧版（2021.1225）支持net4.5/net4.0/net2.0  

|                               项目                               | 年份  |  状态  |  .NET6  | 说明                                                                                 |
| :--------------------------------------------------------------: | :---: | :----: | :-----: | ------------------------------------------------------------------------------------ |
|                             基础组件                             |       |        |         | 支撑其它中间件以及产品项目                                                           |
|          [NewLife.Core](https://github.com/NewLifeX/X)           | 2002  | 维护中 |    √    | 日志、配置、缓存、网络、序列化、APM性能追踪                                     |
|    [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode)    | 2005  | 维护中 |    √    | 大数据中间件，MySQL/SQLite/SqlServer/Oracle/TDengine/达梦，自动分表              |
|      [NewLife.Net](https://github.com/NewLifeX/NewLife.Net)      | 2005  | 维护中 |    √    | 网络库，单机千万级吞吐率（2266万tps），单机百万级连接（400万Tcp）                    |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2005  | 维护中 |    √    | RPC通信框架，内网高吞吐或物联网硬件设备场景                    |
|     [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube)     | 2010  | 维护中 |    √    | 魔方快速开发平台，集成了用户权限、SSO登录、OAuth服务端等，单表100亿级项目验证        |
|    [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent)    | 2008  | 维护中 |    √    | 服务管理组件，把应用安装成为操作系统守护进程，Windows服务、Linux的Systemd            |
|     [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero)     | 2020  | 维护中 |    √    | Zero零代脚手架，各种拷贝即用的项目模板，Web、WebApi、Service      |
|                              中间件                              |       |        |         | 对接知名中间件平台                                                                 |
|    [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis)    | 2017  | 维护中 |    √    | Redis客户端，微秒级延迟，百万级吞吐，丰富的消息队列，百亿级数据量项目验证            |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018  | 维护中 |    √    | 支持Apache RocketMQ和阿里云消息队列，十亿级项目验证                                  |
|     [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT)     | 2019  | 维护中 |    √    | 物联网消息协议，MqttClient/MqttServer，客户端支持阿里云物联网             |
|      [NewLife.IoT](https://github.com/NewLifeX/NewLife.IoT)      | 2022  | 维护中 |    √    | IoT标准库，定义物联网领域的各种通信协议标准规范                                               |
|   [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus)   | 2022  | 维护中 |    √    | 基于IoT标准库实现的ModbusTcp/ModbusRTU协议库，支持IoT平台和IoTEdge                            |
|     [NewLife.LoRa](https://github.com/NewLifeX/NewLife.LoRa)     | 2016  | 维护中 |    √    | 超低功耗的物联网远程通信协议LoRaWAN                                                  |
|                             产品平台                             |       |        |         | 产品平台级，编译部署即用，个性化自定义                                               |
|           [AntJob](https://github.com/NewLifeX/AntJob)           | 2019  | 维护中 |    √    | 蚂蚁调度，分布式大数据计算平台（实时/离线），蚂蚁搬家分片思想，万亿级数据量项目验证  |
|         [Stardust](https://github.com/NewLifeX/Stardust)         | 2018  | 维护中 |    √    | 星尘，分布式服务平台，节点管理、APM监控中心、配置中心、注册中心、发布中心、消息中心  |
|       [CrazyCoder](https://github.com/NewLifeX/XCoder)           | 2006  | 维护中 |    √    | 码神工具，众多开发者工具，网络、串口、加解密、正则表达式、Modbus                     |
|           [XProxy](https://github.com/NewLifeX/XProxy)           | 2005  | 维护中 |    √    | 产品级反向代理，NAT代理、Http代理                                                    |
|        [HttpMeter](https://github.com/NewLifeX/HttpMeter)        | 2022  | 维护中 |    √    | Http压力测试工具                                                    |
|          [SmartOS](https://github.com/NewLifeX/SmartOS)          | 2014  | 维护中 |  C++11  | 嵌入式操作系统，完全独立自主，支持ARM Cortex-M芯片架构                                   |
|         [GitCandy](https://github.com/NewLifeX/GitCandy)         | 2015  | 维护中 |    ×    | Git源代码管理系统                                                                    |
|                           NewLife.A2                           | 2019  |  商用  |    √    | 嵌入式工业计算机，物联网边缘网关，高性能.NET6主机，应用于工业、农业、交通、医疗       |
|                          NewLife.IoT                           | 2020  |  商用  |    √    | 物联网整体解决方案，建筑、环保、农业，软硬件及大数据分析一体化，单机十万级点位项目验证 |
|                          NewLife.UWB                          | 2020  |  商用  |    √    | 厘米级（10~20cm）高精度室内定位，软硬件一体化，与其它系统联动，大型展厅项目验证                 |

## 新生命开发团队
![XCode](https://newlifex.com/logo.png)  

新生命团队（NewLife）成立于2002年，是新时代物联网行业解决方案提供者，致力于提供软硬件应用方案咨询、系统架构规划与开发服务。  
团队主导的开源NewLife系列组件已被广泛应用于各行业，Nuget累计下载量高达60余万次。  
团队开发的大数据核心组件NewLife.XCode、蚂蚁调度计算平台AntJob、星尘分布式平台Stardust、缓存队列组件NewLife.Redis以及物联网平台NewLife.IoT，均成功应用于电力、高校、互联网、电信、交通、物流、工控、医疗、文博等行业，为客户提供了大量先进、可靠、安全、高质量、易扩展的产品和系统集成服务。  

我们将不断通过服务的持续改进，成为客户长期信赖的合作伙伴，通过不断的创新和发展，成为国内优秀的IT服务供应商。  

`新生命团队始于2002年，部分开源项目具有20年以上漫长历史，源码库保留有2010年以来所有修改记录`  
网站：https://newlifex.com  
开源：https://github.com/newlifex  
QQ群：1600800/1600838  
微信公众号：  
![智能大石头](https://newlifex.com/stone.jpg)  
